using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace NukkiStudio.App.Imaging;

/// <summary>
/// 이미지 / 마스크 크기 바꾸기 (해상도 조절, 내보내기 확대).
/// 고품질 바이큐빅으로 늘리거나 줄이고, 확대할 때는 가벼운 언샤프 마스크로 흐려진 윤곽을 되살린다.
/// </summary>
public static class ImageResampler
{
    public static ImageBuffer Resize(ImageBuffer image, int width, int height)
    {
        if (width == image.Width && height == image.Height) return image;
        using var src = BitmapConverter.ToBitmap(image);
        using var dst = ResizeBitmap(src, width, height);
        var result = BitmapConverter.ToBuffer(dst);
        return width > image.Width || height > image.Height ? Sharpen(result, 0.45f) : result;
    }

    /// <summary>고품질 바이큐빅으로 크기를 바꾼 새 32비트 비트맵 (가장자리 번짐 없음).</summary>
    public static Bitmap ResizeBitmap(Bitmap src, int width, int height)
    {
        var dst = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);
        using var g = Graphics.FromImage(dst);
        using var attributes = new ImageAttributes();
        g.CompositingMode = CompositingMode.SourceCopy;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.SmoothingMode = SmoothingMode.HighQuality;
        attributes.SetWrapMode(WrapMode.TileFlipXY);
        g.DrawImage(src, new Rectangle(0, 0, width, height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, attributes);
        return dst;
    }

    /// <summary>확대 저장용: 크기를 바꾸고, 커졌으면 선명하게.</summary>
    public static Bitmap ResizeForExport(Bitmap src, int width, int height)
    {
        var resized = ResizeBitmap(src, width, height);
        if (width <= src.Width && height <= src.Height) return resized;
        using (resized)
        {
            return BitmapConverter.ToBitmap(Sharpen(BitmapConverter.ToBuffer(resized), 0.45f));
        }
    }

    /// <summary>
    /// 언샤프 마스크: 결과 = 원본 + amount × (원본 - 3×3 흐림). 알파는 그대로.
    /// 확대 보간으로 뭉개진 경계를 되살린다 (과하면 테두리가 생기므로 약하게).
    /// </summary>
    public static ImageBuffer Sharpen(ImageBuffer image, float amount)
    {
        int w = image.Width, h = image.Height;
        var src = image.Pixels;
        var dst = (byte[])src.Clone();
        Parallel.For(1, h - 1, y =>
        {
            for (int x = 1; x < w - 1; x++)
            {
                int i = (y * w + x) * 4;
                for (int c = 0; c < 3; c++)
                {
                    int sum = 0;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int row = ((y + dy) * w + x) * 4 + c;
                        sum += src[row - 4] + src[row] + src[row + 4];
                    }
                    float blur = sum / 9f;
                    float v = src[i + c] + amount * (src[i + c] - blur);
                    dst[i + c] = (byte)Math.Clamp((int)(v + 0.5f), 0, 255);
                }
            }
        });
        return new ImageBuffer(w, h, dst);
    }

    /// <summary>마스크 크기 바꾸기 (양선형 보간 — 페더 경계 유지).</summary>
    public static Mask ResizeMask(Mask mask, int width, int height)
    {
        if (width == mask.Width && height == mask.Height) return mask;
        var src = mask.Data;
        int sw = mask.Width, sh = mask.Height;
        var data = new byte[width * height];
        float fx = (float)sw / width, fy = (float)sh / height;
        Parallel.For(0, height, y =>
        {
            float sy = Math.Clamp((y + 0.5f) * fy - 0.5f, 0, sh - 1);
            int y0 = (int)sy, y1 = Math.Min(y0 + 1, sh - 1);
            float ay = sy - y0;
            for (int x = 0; x < width; x++)
            {
                float sx = Math.Clamp((x + 0.5f) * fx - 0.5f, 0, sw - 1);
                int x0 = (int)sx, x1 = Math.Min(x0 + 1, sw - 1);
                float ax = sx - x0;
                float top = src[y0 * sw + x0] + (src[y0 * sw + x1] - src[y0 * sw + x0]) * ax;
                float bottom = src[y1 * sw + x0] + (src[y1 * sw + x1] - src[y1 * sw + x0]) * ax;
                data[y * width + x] = (byte)Math.Clamp((int)(top + (bottom - top) * ay + 0.5f), 0, 255);
            }
        });
        return new Mask(width, height, data);
    }
}
