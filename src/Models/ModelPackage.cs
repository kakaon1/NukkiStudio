namespace NukkiStudio.App.Models;

/// <summary>
/// 내려받을 수 있는 AI 모델 하나.
/// Files의 파일이 models\Folder 안에 모두 있으면 설치된 것으로 본다.
/// Zip이면 압축을 풀어 Files에 적힌 파일만 폴더에 넣는다 (압축 안의 하위 폴더는 무시).
/// </summary>
public sealed record ModelPackage(
    string Id,
    string Title,
    string Purpose,
    string Folder,
    string Url,
    long ApproxBytes,
    bool IsZip,
    string[] Files,
    bool Required)
{
    public string SizeText => FormatSize(ApproxBytes);

    public bool IsInstalled(string modelsDirectory) =>
        Files.All(f => File.Exists(Path.Combine(modelsDirectory, Folder, f)));

    public static string FormatSize(long bytes) =>
        bytes >= 1024L * 1024 * 1024 ? $"{bytes / (1024.0 * 1024 * 1024):0.0} GB" : $"{bytes / (1024.0 * 1024):0.0} MB";
}

/// <summary>프로그램이 쓰는 모델 목록 (출처: Hugging Face, 확인일 2026-10-06).</summary>
public static class ModelPackages
{
    public const string SamId = "mobile_sam";
    public const string DetectorId = "dfine";
    public const string MiGanId = "migan";
    public const string LamaId = "lama";

    public static readonly ModelPackage Sam = new(
        SamId, "MobileSAM", "객체 선택 (클릭 / 박스) — 필수",
        "mobile_sam",
        "https://huggingface.co/nrl-ai/anylearning-labeling-models/resolve/main/mobile_sam_20230629.zip",
        36_700_000, true,
        new[] { "config.yaml", "mobile_sam.encoder.onnx", "sam_vit_h_4b8939.decoder.onnx" },
        Required: true);

    public static readonly ModelPackage Detector = new(
        DetectorId, "D-FINE-N", "객체 자동 선택",
        "dfine",
        "https://huggingface.co/nrl-ai/anylearning-labeling-models/resolve/main/dfine_n_coco_956d170.zip",
        13_900_000, true,
        new[] { "dfine_n_coco.onnx", "LICENSE" },
        Required: false);

    public static readonly ModelPackage MiGan = new(
        MiGanId, "MI-GAN", "객체 지우기 — 빠름",
        "migan",
        "https://huggingface.co/FreeHugsForRobots/ps-inpaint-migan/resolve/main/migan-512-generator.onnx",
        28_098_112, false,
        new[] { "migan-512-generator.onnx" },
        Required: false);

    public static readonly ModelPackage Lama = new(
        LamaId, "LaMa", "객체 지우기 — 고품질 (넓은 영역 / 무늬 배경)",
        "lama",
        "https://huggingface.co/opencv/inpainting_lama/resolve/main/inpainting_lama_2025jan.onnx",
        92_600_000, false,
        new[] { "inpainting_lama_2025jan.onnx" },
        Required: false);

    public static readonly IReadOnlyList<ModelPackage> All = new[] { Sam, Detector, MiGan, Lama };
}
