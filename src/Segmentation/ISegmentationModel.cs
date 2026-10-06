using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Segmentation;

/// <summary>디코더 결과. LowResLogits는 다음 클릭 때 이전 마스크 입력으로 다시 넣는다.</summary>
public sealed record SegmentationResult(Mask Mask, float[] LowResLogits, float Score);

/// <summary>
/// 프롬프트 기반 세그멘테이션 모델 (SAM 계열).
/// SetImageAsync로 이미지를 한 번 인코딩한 뒤, Predict를 클릭마다 호출한다.
/// </summary>
public interface ISegmentationModel : IDisposable
{
    string Name { get; }

    /// <summary>인코더가 실제로 실행되는 장치 설명 (예: "GPU: NVIDIA ..." / "CPU").</summary>
    string DeviceDescription { get; }

    bool HasImage { get; }

    Task SetImageAsync(ImageBuffer image, CancellationToken cancellationToken = default);

    /// <param name="previousLogits">이전 결과의 LowResLogits (없으면 null).</param>
    SegmentationResult Predict(PromptSet prompts, float[]? previousLogits);
}
