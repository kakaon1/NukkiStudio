using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using NukkiStudio.App.Editing;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Imaging;

/// <summary>
/// 원본 위에 겹쳐 그릴 반투명 오버레이를 만든다.
/// 확정된 객체는 각자 색, 현재 선택은 파란색 + 흰 테두리.
/// 브러시 작업 중에는 칠한 영역만 다시 그린다.
/// </summary>
internal static class OverlayRenderer
{
    private static readonly Color SelectionColor = Color.FromArgb(99, 115, 255);
    private const int SelectionAlpha = 120;
    private const int ObjectAlpha = 100;

    /// <summary>선택 마스크 공급: AI 마스크(정리됨) + 브러시 레이어.</summary>
    public readonly record struct SelectionSource(Mask? AiMask, sbyte[]? Brush)
    {
        public byte At(int i)
        {
            if (Brush is { } b && b[i] != 0) return b[i] > 0 ? (byte)255 : (byte)0;
            return AiMask?.Data[i] ?? 0;
        }

        public bool IsEmpty => AiMask is null && Brush is null;
    }

    public static Bitmap Create(int width, int height)
    {
        return new Bitmap(width, height, PixelFormat.Format32bppPArgb);
    }

    public static void Render(Bitmap overlay, Rectangle region, IReadOnlyList<SegmentedObject> objects, SelectionSource selection)
    {
        int w = overlay.Width, h = overlay.Height;
        region.Intersect(new Rectangle(0, 0, w, h));
        if (region.Width <= 0 || region.Height <= 0) return;

        var visible = objects.Where(o => o.Visible).ToArray();
        bool hasSelection = !selection.IsEmpty;
        int rowBytes = region.Width * 4;
        var pixels = new byte[rowBytes * region.Height];

        // 1) 관리 메모리에 병렬로 계산 (비트맵은 잠그지 않는다).
        //    UI 스레드가 병렬 작업을 기다리는 동안 화면 그리기(WM_PAINT)가 끼어들 수 있어,
        //    표시 중인 비트맵을 잠근 채로 기다리면 "Bitmap region is already locked" 오류가 난다 (CODEMAP 5번).
        Parallel.For(0, region.Height, row =>
        {
            int y = region.Y + row;
            int p = row * rowBytes;
            for (int col = 0; col < region.Width; col++, p += 4)
            {
                int x = region.X + col;
                int i = y * w + x;
                int a = 0, r = 0, g = 0, b = 0;

                // 확정 객체 (뒤에 확정된 것이 위)
                foreach (var obj in visible)
                {
                    int m = obj.Mask.Data[i];
                    if (m == 0) continue;
                    int oa = m * ObjectAlpha / 255;
                    Blend(ref a, ref r, ref g, ref b, obj.Color, oa);
                }

                if (hasSelection && selection.At(i) >= 128)
                {
                    bool edge = (x > 0 && selection.At(i - 1) < 128) || (x < w - 1 && selection.At(i + 1) < 128) ||
                                (y > 0 && selection.At(i - w) < 128) || (y < h - 1 && selection.At(i + w) < 128);
                    if (edge) Blend(ref a, ref r, ref g, ref b, Color.White, 255);
                    else Blend(ref a, ref r, ref g, ref b, SelectionColor, SelectionAlpha);
                }

                // 미리 곱한 알파로 기록
                pixels[p + 0] = (byte)(b * a / 255);
                pixels[p + 1] = (byte)(g * a / 255);
                pixels[p + 2] = (byte)(r * a / 255);
                pixels[p + 3] = (byte)a;
            }
        });

        // 2) 잠금은 기다림 없이 복사하는 동안만
        var data = overlay.LockBits(region, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
        try
        {
            for (int row = 0; row < region.Height; row++)
            {
                Marshal.Copy(pixels, row * rowBytes, data.Scan0 + row * data.Stride, rowBytes);
            }
        }
        finally
        {
            overlay.UnlockBits(data);
        }
    }

    /// <summary>(r,g,b,a) 위에 color를 alpha로 덮는다 (직선 알파 기준).</summary>
    private static void Blend(ref int a, ref int r, ref int g, ref int b, Color color, int alpha)
    {
        int outA = alpha + a * (255 - alpha) / 255;
        if (outA == 0) return;
        r = (color.R * alpha + r * a * (255 - alpha) / 255) / outA;
        g = (color.G * alpha + g * a * (255 - alpha) / 255) / outA;
        b = (color.B * alpha + b * a * (255 - alpha) / 255) / outA;
        a = outA;
    }

    /// <summary>마스크를 흑백 표시용 비트맵으로 만든다.</summary>
    public static Bitmap MaskToBitmap(Mask mask)
    {
        var bmp = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppPArgb);
        var data = bmp.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
        try
        {
            unsafe
            {
                IntPtr scan0 = data.Scan0;
                int stride = data.Stride, w = mask.Width;
                var m = mask.Data;
                Parallel.For(0, mask.Height, y =>
                {
                    byte* p = (byte*)scan0 + (long)y * stride;
                    int o = y * w;
                    for (int x = 0; x < w; x++, p += 4)
                    {
                        byte v = m[o + x];
                        p[0] = v; p[1] = v; p[2] = v; p[3] = 255;
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
}
