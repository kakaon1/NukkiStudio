namespace NukkiStudio.App.Imaging;

/// <summary>
/// 32비트 BGRA 픽셀 버퍼 (stride = Width * 4). UI 라이브러리에 의존하지 않는 이미지 표현.
/// </summary>
public sealed class ImageBuffer
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }

    public ImageBuffer(int width, int height)
        : this(width, height, new byte[checked(width * height * 4)])
    {
    }

    public ImageBuffer(int width, int height, byte[] pixels)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (pixels.Length != width * height * 4) throw new ArgumentException("픽셀 버퍼 크기가 맞지 않습니다.", nameof(pixels));
        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public int Stride => Width * 4;
}
