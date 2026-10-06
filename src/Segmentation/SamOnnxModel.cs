using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Segmentation;

/// <summary>
/// SAM(v1) ONNX 형식 모델 (MobileSAM, SAM ViT 등).
/// 인코더: input_image [H, W, 3] RGB float 0~255 → image_embeddings [1, 256, 64, 64]
/// 디코더: image_embeddings, point_coords, point_labels, mask_input, has_mask_input, orig_im_size
///        → masks, iou_predictions, low_res_masks
///
/// 인코더는 내부적으로 입력을 고정 캔버스 크기(예: MobileSAM 1024×682)로 리사이즈한 뒤 1024×1024로 패딩한다.
/// 따라서 이미지를 캔버스 안에 비율을 유지해 왼쪽 위에 넣고 나머지를 0으로 채워 넘긴다.
/// 인코더는 GPU(DirectML) 우선, 디코더는 가볍기 때문에 항상 CPU에서 실행한다.
/// </summary>
public sealed class SamOnnxModel : ISegmentationModel
{
    private const int LowResSize = 256;

    private readonly string _encoderPath;
    private readonly int _canvasW;
    private readonly int _canvasH;
    private InferenceSession _encoder;
    private readonly InferenceSession _decoder;
    private readonly object _sync = new();

    private float[]? _embedding;
    private int[]? _embeddingDims;
    private int _imageW, _imageH, _scaledW, _scaledH;
    private float _scale;

    public string Name { get; }
    public string DeviceDescription { get; private set; }
    public bool HasImage => _embedding is not null;

    public SamOnnxModel(string name, string encoderPath, string decoderPath, int canvasWidth, int canvasHeight, DevicePreference preference)
    {
        Name = name;
        _encoderPath = encoderPath;
        _canvasW = canvasWidth;
        _canvasH = canvasHeight;
        _encoder = OnnxSessionFactory.Create(encoderPath, preference, out var device);
        DeviceDescription = device;
        _decoder = OnnxSessionFactory.CreateCpu(decoderPath, out _);
    }

    public Task SetImageAsync(ImageBuffer image, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_sync)
            {
                _embedding = null;
                _imageW = image.Width;
                _imageH = image.Height;
                _scale = Math.Min((float)_canvasW / image.Width, (float)_canvasH / image.Height);
                _scaledW = Math.Clamp((int)MathF.Round(image.Width * _scale), 1, _canvasW);
                _scaledH = Math.Clamp((int)MathF.Round(image.Height * _scale), 1, _canvasH);
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    Encode(image);
                }
                catch (OnnxRuntimeException) when (DeviceDescription != "CPU")
                {
                    // GPU 실행 실패 → CPU 세션으로 교체 후 재시도
                    _encoder.Dispose();
                    _encoder = OnnxSessionFactory.CreateCpu(_encoderPath, out var device);
                    DeviceDescription = device + " (GPU 실패로 전환)";
                    Encode(image);
                }
            }
        }, cancellationToken);
    }

    private void Encode(ImageBuffer image)
    {
        var data = ImagePreprocessor.ToRgbHwc(image, _scaledW, _scaledH, _canvasW, _canvasH);
        var input = new DenseTensor<float>(data, new[] { _canvasH, _canvasW, 3 });
        string inputName = _encoder.InputMetadata.Keys.First();

        using var results = _encoder.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, input) });
        var output = results.First().AsTensor<float>();
        _embedding = output.ToArray();
        _embeddingDims = output.Dimensions.ToArray();
    }

    public SegmentationResult Predict(PromptSet prompts, float[]? previousLogits)
    {
        if (prompts.IsEmpty) throw new ArgumentException("프롬프트가 비어 있습니다.", nameof(prompts));

        lock (_sync)
        {
            if (_embedding is null || _embeddingDims is null) throw new InvalidOperationException("이미지가 인코딩되지 않았습니다.");

            // 좌표를 캔버스 좌표계로 변환. 라벨: 1 포함, 0 제외, 2/3 박스 모서리, -1 패딩
            var coords = new List<float>();
            var labels = new List<float>();
            foreach (var p in prompts.Points)
            {
                coords.Add(p.X * _scale);
                coords.Add(p.Y * _scale);
                labels.Add(p.Positive ? 1f : 0f);
            }
            if (prompts.Box is { } box)
            {
                coords.Add(box.Left * _scale); coords.Add(box.Top * _scale); labels.Add(2f);
                coords.Add(box.Right * _scale); coords.Add(box.Bottom * _scale); labels.Add(3f);
            }
            else
            {
                coords.Add(0f); coords.Add(0f); labels.Add(-1f);
            }

            int n = labels.Count;
            bool hasMask = previousLogits is not null && previousLogits.Length == LowResSize * LowResSize;
            var maskInput = hasMask ? previousLogits! : new float[LowResSize * LowResSize];

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("image_embeddings", new DenseTensor<float>(_embedding, _embeddingDims)),
                NamedOnnxValue.CreateFromTensor("point_coords", new DenseTensor<float>(coords.ToArray(), new[] { 1, n, 2 })),
                NamedOnnxValue.CreateFromTensor("point_labels", new DenseTensor<float>(labels.ToArray(), new[] { 1, n })),
                NamedOnnxValue.CreateFromTensor("mask_input", new DenseTensor<float>(maskInput, new[] { 1, 1, LowResSize, LowResSize })),
                NamedOnnxValue.CreateFromTensor("has_mask_input", new DenseTensor<float>(new[] { hasMask ? 1f : 0f }, new[] { 1 })),
                // 캔버스 크기를 넘기면 디코더가 1024×1024 패딩을 잘라낸 캔버스 크기 로짓을 돌려준다
                NamedOnnxValue.CreateFromTensor("orig_im_size", new DenseTensor<float>(new[] { (float)_canvasH, _canvasW }, new[] { 2 })),
            };

            using var results = _decoder.Run(inputs);
            var masks = results.First(r => r.Name == "masks").AsTensor<float>();
            var iou = results.First(r => r.Name == "iou_predictions").AsTensor<float>();
            var lowRes = results.First(r => r.Name == "low_res_masks").AsTensor<float>();

            // 마스크가 여러 장이면 점수가 가장 높은 것을 사용
            int count = masks.Dimensions[1];
            int best = 0;
            for (int i = 1; i < count; i++)
            {
                if (iou[0, i] > iou[0, best]) best = i;
            }

            int mh = masks.Dimensions[2], mw = masks.Dimensions[3];
            var logits = new float[mh * mw];
            Array.Copy(masks.ToArray(), best * mh * mw, logits, 0, logits.Length);

            int lh = lowRes.Dimensions[2], lw = lowRes.Dimensions[3];
            var low = new float[lh * lw];
            Array.Copy(lowRes.ToArray(), best * lh * lw, low, 0, low.Length);

            // 캔버스 중 이미지가 들어 있는 왼쪽 위 영역만 원본 크기로 복원
            float fx = (float)mw / _canvasW, fy = (float)mh / _canvasH;
            int regionW = Math.Clamp((int)MathF.Round(_scaledW * fx), 1, mw);
            int regionH = Math.Clamp((int)MathF.Round(_scaledH * fy), 1, mh);
            var mask = MaskOps.FromLogits(logits, mw, regionW, regionH, _imageW, _imageH);
            return new SegmentationResult(mask, low, iou[0, best]);
        }
    }

    public void Dispose()
    {
        _encoder.Dispose();
        _decoder.Dispose();
    }
}
