using System.Drawing;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Inpainting;

/// <summary>
/// 512×512 입력 인페인팅 모델(MI-GAN / LaMa)의 공통 처리.
/// 1) 지울 영역을 조금 넓힌다 (외곽선 / 그림자 잔상까지 지우도록)
/// 2) 주변 맥락을 포함한 정사각형을 잘라 512로 맞춰 모델에 넣는다 (작은 영역은 512 그대로 = 원본 해상도)
/// 3) 결과를 원래 크기로 되돌려 지울 영역에만 부드럽게 합성한다
/// 4) 크게 줄였다 늘린 경우에는 주변과 같은 세기의 입자(노이즈)를 더해 "매끈하게 비어 보이는" 느낌을 줄인다
/// </summary>
public abstract class CropInpainter : IInpainter
{
    protected const int Size = 512;
    protected const int Plane = Size * Size;

    public abstract string Name { get; }

    /// <summary>
    /// rgb: 512×512 HWC RGB (0~255), known: 512×512 (1 = 남길 영역, 0 = 지울 영역).
    /// 반환: CHW RGB (0~255) 3×512×512.
    /// </summary>
    protected abstract float[] Run(float[] rgb, float[] known);

    public ImageBuffer Inpaint(ImageBuffer image, Mask hole)
    {
        int w = image.Width, h = image.Height;
        var dilated = MaskOps.Grow(BinaryOf(hole), Math.Max(4, Math.Max(w, h) / 150));
        var bounds = dilated.GetBounds();
        if (bounds.IsEmpty) return image;

        var crop = ContextCrop(bounds, w, h);
        var rgb = ImagePreprocessor.ToRgbHwc(Crop(image, crop), Size, Size, Size, Size);
        var known = SampleMaskNearest(dilated, crop);
        var output = Run(rgb, known);

        // 512보다 크게 잘랐으면 결과가 늘어나며 흐려진다 → 주변 질감 세기만큼 입자를 더한다
        float scale = Math.Max(crop.Width, crop.Height) / (float)Size;
        float grainAmount = Math.Clamp((scale - 1.2f) / 1.3f, 0f, 1f);
        float[] grainStd = grainAmount > 0 ? MeasureGrain(image, dilated, crop) : new float[3];

        var blend = MaskOps.Feather(dilated, Math.Max(2, Math.Max(w, h) / 600));
        var result = new ImageBuffer(w, h, (byte[])image.Pixels.Clone());
        var dst = result.Pixels;
        Parallel.For(0, crop.Height, row =>
        {
            int y = crop.Y + row;
            float sy = (row + 0.5f) * Size / crop.Height - 0.5f;
            var random = new Random(y * 7919 + crop.X);
            for (int col = 0; col < crop.Width; col++)
            {
                int x = crop.X + col;
                int a = blend.Data[y * w + x];
                if (a == 0) continue;
                float sx = (col + 0.5f) * Size / crop.Width - 0.5f;
                float noise = grainAmount > 0 ? Gaussian(random) * grainAmount : 0f;
                int di = (y * w + x) * 4;
                for (int c = 0; c < 3; c++)
                {
                    float v = Bilinear(output, c * Plane, sx, sy) + noise * grainStd[c];
                    byte gen = (byte)Math.Clamp((int)(v + 0.5f), 0, 255);
                    // BGRA 순서: c=0(R) → 2, c=1(G) → 1, c=2(B) → 0
                    int bi = di + (2 - c);
                    dst[bi] = (byte)((gen * a + dst[bi] * (255 - a)) / 255);
                }
                dst[di + 3] = (byte)Math.Max(dst[di + 3], a);
            }
        });
        return result;
    }

    /// <summary>지울 영역 경계 사각형 둘레에 주변 맥락을 포함한 정사각형 (최소 512, 이미지 안으로 제한).</summary>
    private static Rectangle ContextCrop(Rectangle bounds, int w, int h)
    {
        int side = Math.Max(Math.Max(bounds.Width, bounds.Height) * 2, Size);
        side = Math.Min(side, Math.Max(w, h));
        int cw = Math.Min(side, w), ch = Math.Min(side, h);
        int cx = bounds.X + bounds.Width / 2, cy = bounds.Y + bounds.Height / 2;
        int x = Math.Clamp(cx - cw / 2, 0, w - cw);
        int y = Math.Clamp(cy - ch / 2, 0, h - ch);
        return new Rectangle(x, y, cw, ch);
    }

    private static ImageBuffer Crop(ImageBuffer image, Rectangle r)
    {
        var dst = new ImageBuffer(r.Width, r.Height);
        for (int row = 0; row < r.Height; row++)
        {
            Buffer.BlockCopy(image.Pixels, ((r.Y + row) * image.Width + r.X) * 4, dst.Pixels, row * r.Width * 4, r.Width * 4);
        }
        return dst;
    }

    /// <summary>잘라낸 영역의 마스크를 512×512로 샘플링. 지울 영역이 하나라도 걸리면 0 (보수적으로 지움).</summary>
    private static float[] SampleMaskNearest(Mask hole, Rectangle crop)
    {
        var known = new float[Plane];
        float fx = (float)crop.Width / Size, fy = (float)crop.Height / Size;
        Parallel.For(0, Size, y =>
        {
            int y0 = crop.Y + (int)(y * fy), y1 = Math.Max(y0 + 1, crop.Y + (int)((y + 1) * fy));
            for (int x = 0; x < Size; x++)
            {
                int x0 = crop.X + (int)(x * fx), x1 = Math.Max(x0 + 1, crop.X + (int)((x + 1) * fx));
                bool isHole = false;
                for (int yy = y0; yy < y1 && !isHole; yy++)
                    for (int xx = x0; xx < x1; xx++)
                        if (hole.Data[yy * hole.Width + xx] >= 128) { isHole = true; break; }
                known[y * Size + x] = isHole ? 0f : 1f;
            }
        });
        return known;
    }

    /// <summary>
    /// 지울 영역 바로 바깥 띠에서 잔결(고주파) 세기를 채널별 표준편차로 잰다.
    /// 잔결 = 픽셀 - 상하좌우 2픽셀 떨어진 이웃 평균.
    /// </summary>
    private static float[] MeasureGrain(ImageBuffer image, Mask hole, Rectangle crop)
    {
        int w = image.Width;
        var ring = MaskOps.Grow(hole, Math.Max(6, crop.Width / 25));
        var px = image.Pixels;
        double[] sum = new double[3], sumSq = new double[3];
        long count = 0;
        for (int y = crop.Y + 2; y < crop.Bottom - 2; y += 2)
        {
            for (int x = crop.X + 2; x < crop.Right - 2; x += 2)
            {
                int i = y * w + x;
                if (hole.Data[i] >= 128 || ring.Data[i] < 128) continue;
                for (int c = 0; c < 3; c++)
                {
                    int v = px[i * 4 + c];
                    float neighbors = (px[(i - 2) * 4 + c] + px[(i + 2) * 4 + c] + px[(i - 2 * w) * 4 + c] + px[(i + 2 * w) * 4 + c]) / 4f;
                    double r = v - neighbors;
                    sum[c] += r;
                    sumSq[c] += r * r;
                }
                count++;
            }
        }
        var std = new float[3];
        if (count < 16) return std;
        // BGRA 순서로 잰 값을 RGB 순서로 돌려준다
        for (int c = 0; c < 3; c++)
        {
            double mean = sum[c] / count;
            float s = (float)Math.Sqrt(Math.Max(0, sumSq[c] / count - mean * mean));
            std[2 - c] = Math.Min(s * 0.8f, 24f);
        }
        return std;
    }

    private static float Gaussian(Random random)
    {
        double u1 = 1.0 - random.NextDouble(), u2 = random.NextDouble();
        return (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2));
    }

    private static float Bilinear(float[] data, int offset, float x, float y)
    {
        x = Math.Clamp(x, 0, Size - 1);
        y = Math.Clamp(y, 0, Size - 1);
        int x0 = (int)x, y0 = (int)y;
        int x1 = Math.Min(x0 + 1, Size - 1), y1 = Math.Min(y0 + 1, Size - 1);
        float ax = x - x0, ay = y - y0;
        float top = data[offset + y0 * Size + x0] + (data[offset + y0 * Size + x1] - data[offset + y0 * Size + x0]) * ax;
        float bottom = data[offset + y1 * Size + x0] + (data[offset + y1 * Size + x1] - data[offset + y1 * Size + x0]) * ax;
        return top + (bottom - top) * ay;
    }

    internal static Mask BinaryOf(Mask m)
    {
        var data = new byte[m.Data.Length];
        for (int i = 0; i < data.Length; i++) data[i] = m.Data[i] >= 128 ? (byte)255 : (byte)0;
        return new Mask(m.Width, m.Height, data);
    }

    public abstract void Dispose();
}
