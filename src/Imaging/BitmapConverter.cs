using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Imaging;

/// <summary>System.Drawing.Bitmap ↔ ImageBuffer 변환과 이미지 파일 읽기.</summary>
internal static class BitmapConverter
{
    public static readonly string OpenFilter =
        "이미지 파일|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|모든 파일|*.*";

    /// <summary>파일에서 이미지를 읽어 EXIF 회전을 적용한 32비트 비트맵을 만든다. 파일 잠금을 남기지 않는다.</summary>
    public static Bitmap LoadImageFile(string path)
    {
        using var stream = new MemoryStream(File.ReadAllBytes(path));
        using var image = Image.FromStream(stream);
        ApplyExifOrientation(image);
        return ToArgbBitmap(image);
    }

    public static Bitmap ToArgbBitmap(Image image)
    {
        var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        bmp.SetResolution(96, 96);
        using var g = Graphics.FromImage(bmp);
        g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
        return bmp;
    }

    private static void ApplyExifOrientation(Image image)
    {
        const int OrientationId = 0x0112;
        if (!image.PropertyIdList.Contains(OrientationId)) return;
        var prop = image.GetPropertyItem(OrientationId);
        if (prop?.Value is not { Length: > 0 } value) return;

        var flip = value[0] switch
        {
            2 => RotateFlipType.RotateNoneFlipX,
            3 => RotateFlipType.Rotate180FlipNone,
            4 => RotateFlipType.Rotate180FlipX,
            5 => RotateFlipType.Rotate90FlipX,
            6 => RotateFlipType.Rotate90FlipNone,
            7 => RotateFlipType.Rotate270FlipX,
            8 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone,
        };
        if (flip != RotateFlipType.RotateNoneFlipNone) image.RotateFlip(flip);
    }

    public static ImageBuffer ToBuffer(Bitmap bitmap)
    {
        var buffer = new ImageBuffer(bitmap.Width, bitmap.Height);
        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            CopyRows(data.Scan0, data.Stride, buffer.Pixels, buffer.Stride, buffer.Height, toManaged: true);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
        return buffer;
    }

    /// <summary>저장 / 클립보드용 (일반 ARGB).</summary>
    public static Bitmap ToBitmap(ImageBuffer buffer)
    {
        var bmp = new Bitmap(buffer.Width, buffer.Height, PixelFormat.Format32bppArgb);
        var data = bmp.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            CopyRows(data.Scan0, data.Stride, buffer.Pixels, buffer.Stride, buffer.Height, toManaged: false);
        }
        finally
        {
            bmp.UnlockBits(data);
        }
        return bmp;
    }

    /// <summary>화면 표시용 (미리 곱한 알파 PArgb — GDI+ 그리기가 가장 빠름).</summary>
    public static Bitmap ToDisplayBitmap(ImageBuffer buffer)
    {
        var bmp = new Bitmap(buffer.Width, buffer.Height, PixelFormat.Format32bppPArgb);
        var data = bmp.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
        try
        {
            unsafe
            {
                var src = buffer.Pixels;
                int w = buffer.Width;
                IntPtr scan0 = data.Scan0;
                int stride = data.Stride;
                Parallel.For(0, buffer.Height, y =>
                {
                    byte* row = (byte*)scan0 + (long)y * stride;
                    int si = y * w * 4;
                    for (int x = 0; x < w; x++, si += 4)
                    {
                        int a = src[si + 3];
                        row[x * 4 + 0] = (byte)(src[si + 0] * a / 255);
                        row[x * 4 + 1] = (byte)(src[si + 1] * a / 255);
                        row[x * 4 + 2] = (byte)(src[si + 2] * a / 255);
                        row[x * 4 + 3] = (byte)a;
                    }
                });
            }
        }
        finally
        {
            bmp.UnlockBits(data);
        }
        return bmp;
    }

    private static void CopyRows(IntPtr scan0, int bitmapStride, byte[] managed, int managedStride, int height, bool toManaged)
    {
        for (int y = 0; y < height; y++)
        {
            var ptr = scan0 + y * bitmapStride;
            if (toManaged) Marshal.Copy(ptr, managed, y * managedStride, managedStride);
            else Marshal.Copy(managed, y * managedStride, ptr, managedStride);
        }
    }
}
