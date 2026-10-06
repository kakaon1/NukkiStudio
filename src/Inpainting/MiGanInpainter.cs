using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NukkiStudio.App.Segmentation;

namespace NukkiStudio.App.Inpainting;

/// <summary>
/// MI-GAN (Picsart AI Research, MIT) 512 생성기 ONNX — 빠른 지우기.
/// 입력 gen_input [1, 4, 512, 512] = [mask - 0.5, RGB(-1~1) × mask] (mask: 1 = 남길 영역, 0 = 지울 영역)
/// 출력 gen_output [1, 3, 512, 512] RGB (-1~1)
/// 자르기 / 합성은 CropInpainter가 담당한다.
/// </summary>
public sealed class MiGanInpainter : CropInpainter
{
    public const string ModelFileName = "migan-512-generator.onnx";

    private readonly InferenceSession _session;
    private readonly string _inputName;

    public override string Name { get; }

    public MiGanInpainter(string modelPath, DevicePreference preference)
    {
        _session = OnnxSessionFactory.Create(modelPath, preference, out var device);
        _inputName = _session.InputMetadata.Keys.First();
        Name = $"MI-GAN · {device}";
    }

    protected override float[] Run(float[] rgb, float[] known)
    {
        var input = new float[4 * Plane];
        for (int i = 0; i < Plane; i++)
        {
            float m = known[i];
            input[i] = m - 0.5f;
            input[Plane + i] = (rgb[i * 3] / 127.5f - 1f) * m;
            input[2 * Plane + i] = (rgb[i * 3 + 1] / 127.5f - 1f) * m;
            input[3 * Plane + i] = (rgb[i * 3 + 2] / 127.5f - 1f) * m;
        }

        using var results = _session.Run(new[] { NamedOnnxValue.CreateFromTensor(_inputName, new DenseTensor<float>(input, new[] { 1, 4, Size, Size })) });
        var output = results.First().AsTensor<float>().ToArray();
        for (int i = 0; i < output.Length; i++) output[i] = (output[i] + 1f) * 127.5f;
        return output;
    }

    public override void Dispose() => _session.Dispose();
}
