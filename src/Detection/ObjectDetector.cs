using System.Drawing;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NukkiStudio.App.Imaging;
using NukkiStudio.App.Segmentation;

namespace NukkiStudio.App.Detection;

/// <summary>감지된 객체 하나 (원본 이미지 좌표).</summary>
public sealed record Detection(RectangleF Box, int ClassId, string Label, float Score);

/// <summary>
/// D-FINE-N (COCO 80종, Apache 2.0) ONNX로 이미지 속 객체를 자동으로 찾는다.
/// 입력: images [1,3,640,640] RGB/255 (640×640으로 늘려서), orig_target_sizes int64 [1,2] = [너비, 높이]
/// 출력: labels int64 [1,300], boxes [1,300,4] (원본 xyxy), scores [1,300]
/// 찾은 상자는 SAM의 박스 프롬프트로 넘겨 정확한 윤곽(마스크)을 만든다.
/// </summary>
public sealed class ObjectDetector : IDisposable
{
    public const string ModelFileName = "dfine_n_coco.onnx";
    private const int Size = 640;

    private readonly InferenceSession _session;

    public string DeviceDescription { get; }

    /// <remarks>
    /// DirectML(GPU)에서는 D-FINE의 일부 연산 결과가 틀려 점수가 크게 낮아진다 (CODEMAP.md 3번).
    /// 감지는 CPU에서도 충분히 빠르므로 항상 CPU로 실행한다.
    /// </remarks>
    public ObjectDetector(string modelPath)
    {
        _session = OnnxSessionFactory.CreateCpu(modelPath, out var device);
        DeviceDescription = device;
    }

    /// <param name="minScore">이 점수 미만은 버린다.</param>
    /// <param name="maxCount">점수 높은 순으로 최대 개수.</param>
    public List<Detection> Detect(ImageBuffer image, float minScore = 0.5f, int maxCount = 20)
    {
        var rgb = ImagePreprocessor.ToRgbHwc(image, Size, Size, Size, Size);
        var input = new float[3 * Size * Size];
        int plane = Size * Size;
        for (int i = 0; i < plane; i++)
        {
            input[i] = rgb[i * 3] / 255f;
            input[plane + i] = rgb[i * 3 + 1] / 255f;
            input[2 * plane + i] = rgb[i * 3 + 2] / 255f;
        }

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("images", new DenseTensor<float>(input, new[] { 1, 3, Size, Size })),
            NamedOnnxValue.CreateFromTensor("orig_target_sizes", new DenseTensor<long>(new long[] { image.Width, image.Height }, new[] { 1, 2 })),
        };

        using var results = _session.Run(inputs);
        var labels = results.First(r => r.Name == "labels").AsTensor<long>();
        var boxes = results.First(r => r.Name == "boxes").AsTensor<float>();
        var scores = results.First(r => r.Name == "scores").AsTensor<float>();

        var candidates = new List<Detection>();
        int count = scores.Dimensions[1];
        float minArea = image.Width * image.Height * 0.0015f; // 너무 작은 상자 제외
        for (int i = 0; i < count; i++)
        {
            float score = scores[0, i];
            if (score < minScore) continue;
            var box = RectangleF.FromLTRB(
                Math.Clamp(boxes[0, i, 0], 0, image.Width), Math.Clamp(boxes[0, i, 1], 0, image.Height),
                Math.Clamp(boxes[0, i, 2], 0, image.Width), Math.Clamp(boxes[0, i, 3], 0, image.Height));
            if (box.Width * box.Height < minArea) continue;
            int cls = (int)labels[0, i];
            candidates.Add(new Detection(box, cls, CocoLabels.Korean(cls), score));
        }

        // 같은 종류끼리 많이 겹치는 상자는 점수 높은 것만 남긴다 (NMS)
        var kept = new List<Detection>();
        foreach (var d in candidates.OrderByDescending(d => d.Score))
        {
            if (kept.Any(k => k.ClassId == d.ClassId && IoU(k.Box, d.Box) > 0.6f)) continue;
            kept.Add(d);
            if (kept.Count >= maxCount) break;
        }
        return kept;
    }

    private static float IoU(RectangleF a, RectangleF b)
    {
        var i = RectangleF.Intersect(a, b);
        if (i.IsEmpty) return 0;
        float inter = i.Width * i.Height;
        return inter / (a.Width * a.Height + b.Width * b.Height - inter);
    }

    public void Dispose() => _session.Dispose();
}
