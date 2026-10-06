using System.Drawing;

namespace NukkiStudio.App.Imaging;

/// <summary>
/// 마스크를 알파 채널로 적용해 누끼 결과 이미지를 만든다.
/// </summary>
public static class Compositor
{
    /// <summary>
    /// image에 mask를 알파로 적용한다. crop이 지정되면 그 영역만 잘라낸다.
    /// background가 지정되면 투명 대신 그 색 위에 합성한다 (결과는 불투명).
    /// </summary>
    public static ImageBuffer Cutout(ImageBuffer image, Mask mask, Rectangle? crop = null, Color? background = null)
    {
        if (image.Width != mask.Width || image.Height != mask.Height)
            throw new ArgumentException("이미지와 마스크 크기가 다릅니다.");

        var area = crop ?? new Rectangle(0, 0, image.Width, image.Height);
        area.Intersect(new Rectangle(0, 0, image.Width, image.Height));
        if (area.Width <= 0 || area.Height <= 0) throw new ArgumentException("자를 영역이 비어 있습니다.");

        var dst = new ImageBuffer(area.Width, area.Height);
        var src = image.Pixels;
        var m = mask.Data;
        var d = dst.Pixels;
        int srcW = image.Width;

        Parallel.For(0, area.Height, row =>
        {
            int sy = area.Y + row;
            for (int col = 0; col < area.Width; col++)
            {
                int sx = area.X + col;
                int si = (sy * srcW + sx) * 4;
                int di = (row * area.Width + col) * 4;
                int a = m[sy * srcW + sx] * src[si + 3] / 255;

                if (background is Color bg)
                {
                    d[di + 0] = (byte)((src[si + 0] * a + bg.B * (255 - a)) / 255);
                    d[di + 1] = (byte)((src[si + 1] * a + bg.G * (255 - a)) / 255);
                    d[di + 2] = (byte)((src[si + 2] * a + bg.R * (255 - a)) / 255);
                    d[di + 3] = 255;
                }
                else
                {
                    d[di + 0] = src[si + 0];
                    d[di + 1] = src[si + 1];
                    d[di + 2] = src[si + 2];
                    d[di + 3] = (byte)a;
                }
            }
        });
        return dst;
    }

    /// <summary>경계 사각형에 여백을 더하고 이미지 범위로 제한한다.</summary>
    public static Rectangle Inflate(Rectangle bounds, int margin, int width, int height)
    {
        var r = Rectangle.Inflate(bounds, margin, margin);
        r.Intersect(new Rectangle(0, 0, width, height));
        return r;
    }
}
