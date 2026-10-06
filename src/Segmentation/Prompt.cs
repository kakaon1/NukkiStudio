using System.Drawing;

namespace NukkiStudio.App.Segmentation;

/// <summary>객체 지정 점. Positive = true 이면 포함 점, false 이면 제외 점. 좌표는 원본 이미지 픽셀 기준.</summary>
public readonly record struct PromptPoint(float X, float Y, bool Positive);

/// <summary>디코더에 넘길 프롬프트 묶음 (점 여러 개 + 박스 0~1개).</summary>
public sealed record PromptSet(IReadOnlyList<PromptPoint> Points, RectangleF? Box)
{
    public bool IsEmpty => Points.Count == 0 && Box is null;
}
