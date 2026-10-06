using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NukkiStudio.App.Segmentation;

namespace NukkiStudio.App.Inpainting;

/// <summary>
/// LaMa (Samsung AI, Apache 2.0) ONNX — 고품질 지우기. 넓은 영역 / 반복 무늬 배경을 MI-GAN보다 자연스럽게 채운다.
/// 입력 image [1, 3, 512, 512] RGB (0~1), mask [1, 1, 512, 512] (1 = 지울 영역)
/// 출력 [1, 3, 512, 512] RGB — 모델 변환본에 따라 0~1 또는 0~255라서 값 범위를 보고 맞춘다.
/// 항상 CPU로 실행한다 (DirectML 미지원 연산, CODEMAP 6번). 512×512 기준 약 2초.
/// </summary>
public sealed class LamaInpainter : CropInpainter
{
    public const string ModelFileName = "inpainting_lama_2025jan.onnx";

    /// <summary>폴더에서 쓸 LaMa 모델 파일을 찾는다 (없으면 null).</summary>
    public static string? FindModel(string folder)
    {
        var path = Path.Combine(folder, ModelFileName);
        return File.Exists(path) ? path : null;
    }

    private readonly string _modelPath;
    private InferenceSession _session;
    private string _device;
    private readonly string _imageName;
    private readonly string _maskName;

    public override string Name => $"LaMa · {_device}";

    public LamaInpainter(string modelPath, DevicePreference preference)
    {
        _modelPath = modelPath;
        // 항상 CPU: DirectML은 LaMa의 푸리에 합성곱(FFC) 부분 MatMul을 실행하지 못한다 (CODEMAP 6번).
        // GPU 세션을 만들었다가 매번 실패하고 CPU로 다시 만드는 시간을 없앤다. (preference는 다른 모델과 같은 형태로 받기만 함)
        _ = preference;
        _session = OnnxSessionFactory.CreateCpu(modelPath, out _device);
        var inputs = _session.InputMetadata.Keys.ToList();
        _maskName = inputs.FirstOrDefault(n => n.Contains("mask", StringComparison.OrdinalIgnoreCase)) ?? inputs[^1];
        _imageName = inputs.First(n => n != _maskName);
    }

    protected override float[] Run(float[] rgb, float[] known)
    {
        var image = new float[3 * Plane];
        var mask = new float[Plane];
        for (int i = 0; i < Plane; i++)
        {
            image[i] = rgb[i * 3] / 255f;
            image[Plane + i] = rgb[i * 3 + 1] / 255f;
            image[2 * Plane + i] = rgb[i * 3 + 2] / 255f;
            mask[i] = known[i] < 0.5f ? 1f : 0f;
        }

        float[] output;
        try
        {
            output = Infer(image, mask);
        }
        catch (OnnxRuntimeException) when (_device != "CPU")
        {
            // GPU(DirectML)에서 지원하지 않는 연산이면 CPU로 바꿔 다시 실행
            _session.Dispose();
            _session = OnnxSessionFactory.CreateCpu(_modelPath, out _device);
            output = Infer(image, mask);
        }

        // 0~1 출력이면 0~255로
        float max = 0;
        for (int i = 0; i < output.Length; i += 97) max = Math.Max(max, output[i]);
        if (max <= 1.5f)
        {
            for (int i = 0; i < output.Length; i++) output[i] *= 255f;
        }
        return output;
    }

    private float[] Infer(float[] image, float[] mask)
    {
        var inputs = new[]
        {
            NamedOnnxValue.CreateFromTensor(_imageName, new DenseTensor<float>(image, new[] { 1, 3, Size, Size })),
            NamedOnnxValue.CreateFromTensor(_maskName, new DenseTensor<float>(mask, new[] { 1, 1, Size, Size })),
        };
        using var results = _session.Run(inputs);
        return results.First().AsTensor<float>().ToArray();
    }

    public override void Dispose() => _session.Dispose();
}
