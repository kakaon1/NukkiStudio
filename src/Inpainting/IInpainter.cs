using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Inpainting;

/// <summary>
/// 지울 영역(hole)을 주변 배경으로 자연스럽게 채운다 (객체 지우기).
/// </summary>
public interface IInpainter : IDisposable
{
    string Name { get; }

    /// <summary>hole: 255 = 지울 영역. 원본은 바꾸지 않고 새 이미지를 반환한다.</summary>
    ImageBuffer Inpaint(ImageBuffer image, Mask hole);
}
