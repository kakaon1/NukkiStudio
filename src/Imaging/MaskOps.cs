using System.Drawing;

namespace NukkiStudio.App.Imaging;

/// <summary>
/// 마스크 후처리: 로짓 → 이진 마스크, 작은 조각 제거 / 구멍 메우기, 확장 / 축소, 페더, 합집합.
/// 모든 연산은 새 마스크를 반환한다.
/// </summary>
public static class MaskOps
{
    /// <summary>
    /// 로짓(logitW × logitH)을 바이리니어로 outW × outH 크기에 맞춰 샘플링하고 0을 기준으로 이진화한다.
    /// </summary>
    public static Mask FromLogits(float[] logits, int logitW, int logitH, int outW, int outH)
        => FromLogits(logits, logitW, logitW, logitH, outW, outH);

    /// <summary>
    /// 한 줄 폭이 stride인 로짓 배열의 왼쪽 위 logitW × logitH 영역만 사용해 outW × outH로 샘플링한다.
    /// </summary>
    public static Mask FromLogits(float[] logits, int stride, int logitW, int logitH, int outW, int outH)
    {
        var mask = new Mask(outW, outH);
        var data = mask.Data;

        // 열별 샘플 위치를 미리 계산
        var x0 = new int[outW];
        var x1 = new int[outW];
        var fx = new float[outW];
        float sx = (float)logitW / outW;
        for (int x = 0; x < outW; x++)
        {
            float src = (x + 0.5f) * sx - 0.5f;
            if (src < 0) src = 0;
            int i0 = (int)src;
            if (i0 >= logitW - 1) { i0 = logitW - 1; x0[x] = i0; x1[x] = i0; fx[x] = 0; continue; }
            x0[x] = i0;
            x1[x] = i0 + 1;
            fx[x] = src - i0;
        }

        float sy = (float)logitH / outH;
        Parallel.For(0, outH, y =>
        {
            float src = (y + 0.5f) * sy - 0.5f;
            if (src < 0) src = 0;
            int j0 = (int)src;
            int j1 = j0 + 1;
            float fy = src - j0;
            if (j0 >= logitH - 1) { j0 = logitH - 1; j1 = j0; fy = 0; }

            int r0 = j0 * stride;
            int r1 = j1 * stride;
            int o = y * outW;
            for (int x = 0; x < outW; x++)
            {
                float a = logits[r0 + x0[x]];
                float b = logits[r0 + x1[x]];
                float c = logits[r1 + x0[x]];
                float d = logits[r1 + x1[x]];
                float top = a + (b - a) * fx[x];
                float bottom = c + (d - c) * fx[x];
                float v = top + (bottom - top) * fy;
                data[o + x] = v > 0f ? (byte)255 : (byte)0;
            }
        });
        return mask;
    }

    /// <summary>
    /// 이진 마스크에서 작은 객체 조각을 지우고, 객체 안의 작은 구멍을 메운다.
    /// 가장 큰 조각의 ratio 미만인 조각과, 객체 면적의 ratio 미만인 구멍(테두리에 닿지 않는 배경)이 대상이다.
    /// </summary>
    public static Mask Cleanup(Mask src, float ratio = 0.01f, int minPixels = 64)
    {
        int w = src.Width, h = src.Height;
        var data = (byte[])src.Data.Clone();
        var labels = new int[w * h];
        var stack = new Stack<int>();

        // 1) 객체 조각
        var areas = new List<int> { 0 };
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] < 128 || labels[i] != 0) continue;
            int id = areas.Count;
            areas.Add(Flood(data, labels, w, h, i, id, fg: true, stack, out _));
        }
        int largest = areas.Count > 1 ? areas.Max() : 0;
        int minArea = Math.Max(minPixels, (int)(largest * ratio));
        int fgArea = 0;
        for (int i = 0; i < data.Length; i++)
        {
            int id = labels[i];
            if (id == 0) continue;
            if (areas[id] < minArea && areas[id] != largest) data[i] = 0;
            else fgArea++;
        }

        // 2) 구멍 (배경 조각)
        Array.Clear(labels);
        int maxHole = Math.Max(minPixels, (int)(fgArea * ratio));
        var holeIds = new List<bool> { false };
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] >= 128 || labels[i] != 0) continue;
            int id = holeIds.Count;
            int area = Flood(data, labels, w, h, i, id, fg: false, stack, out bool touchesBorder);
            holeIds.Add(!touchesBorder && area <= maxHole);
        }
        for (int i = 0; i < data.Length; i++)
        {
            int id = labels[i];
            if (id != 0 && holeIds[id]) data[i] = 255;
        }

        return new Mask(w, h, data);
    }

    /// <summary>
    /// 프롬프트와 연결된 조각만 남긴다: 포함 점이 들어 있는 조각, 또는 (점이 없으면) 박스 안에 중심이 있는 조각.
    /// SAM이 클릭한 객체 옆의 다른 물체 일부를 함께 잡는 경우를 막는다. 남길 조각이 없으면 원본을 그대로 돌려준다.
    /// </summary>
    public static Mask KeepPromptedRegions(Mask src, IEnumerable<PointF> positivePoints, RectangleF? box)
    {
        int w = src.Width, h = src.Height;
        var labels = new int[w * h];
        var stack = new Stack<int>();
        var areas = new List<int> { 0 };
        var sumX = new List<long> { 0 };
        var sumY = new List<long> { 0 };
        for (int i = 0; i < src.Data.Length; i++)
        {
            if (src.Data[i] < 128 || labels[i] != 0) continue;
            int id = areas.Count;
            areas.Add(Flood(src.Data, labels, w, h, i, id, fg: true, stack, out _));
            sumX.Add(0);
            sumY.Add(0);
        }
        if (areas.Count <= 2) return src; // 조각이 하나뿐이면 그대로

        var keep = new bool[areas.Count];
        bool any = false;
        var points = positivePoints.ToList();
        // 점이 경계 바로 바깥에 찍혀도 가장 가까운 조각을 찾는다 (반경: 긴 변의 2%, 최소 8px)
        int radius = Math.Max(8, Math.Max(w, h) / 50);
        foreach (var p in points)
        {
            int px = (int)p.X, py = (int)p.Y;
            if (px < 0 || py < 0 || px >= w || py >= h) continue;
            int id = labels[py * w + px];
            if (id == 0)
            {
                long best = long.MaxValue;
                for (int y = Math.Max(0, py - radius); y <= Math.Min(h - 1, py + radius); y++)
                {
                    for (int x = Math.Max(0, px - radius); x <= Math.Min(w - 1, px + radius); x++)
                    {
                        int l = labels[y * w + x];
                        if (l == 0) continue;
                        long d = (long)(x - px) * (x - px) + (long)(y - py) * (y - py);
                        if (d < best) { best = d; id = l; }
                    }
                }
            }
            if (id > 0) { keep[id] = true; any = true; }
        }

        if (!any && box is { } b)
        {
            // 박스 프롬프트: 무게중심이 박스 안에 있는 조각
            for (int i = 0; i < labels.Length; i++)
            {
                int id = labels[i];
                if (id == 0) continue;
                sumX[id] += i % w;
                sumY[id] += i / w;
            }
            for (int id = 1; id < areas.Count; id++)
            {
                float cx = (float)sumX[id] / areas[id], cy = (float)sumY[id] / areas[id];
                if (b.Contains(cx, cy)) { keep[id] = true; any = true; }
            }
        }
        if (!any) return src;

        var dst = new Mask(w, h);
        for (int i = 0; i < labels.Length; i++)
        {
            if (keep[labels[i]] && labels[i] != 0) dst.Data[i] = src.Data[i];
        }
        return dst;
    }

    private static int Flood(byte[] data, int[] labels, int w, int h, int start, int id, bool fg, Stack<int> stack, out bool touchesBorder)
    {
        touchesBorder = false;
        int area = 0;
        labels[start] = id;
        stack.Push(start);
        while (stack.Count > 0)
        {
            int p = stack.Pop();
            area++;
            int x = p % w, y = p / w;
            if (x == 0 || y == 0 || x == w - 1 || y == h - 1) touchesBorder = true;

            if (x > 0) Visit(p - 1);
            if (x < w - 1) Visit(p + 1);
            if (y > 0) Visit(p - w);
            if (y < h - 1) Visit(p + w);
        }
        return area;

        void Visit(int q)
        {
            if (labels[q] != 0) return;
            if ((data[q] >= 128) != fg) return;
            labels[q] = id;
            stack.Push(q);
        }
    }

    /// <summary>radius &gt; 0 이면 확장(dilate), &lt; 0 이면 축소(erode). 정사각형 구조 요소.</summary>
    public static Mask Grow(Mask src, int radius)
    {
        if (radius == 0) return src;
        int w = src.Width, h = src.Height;
        int r = Math.Abs(radius);
        bool dilate = radius > 0;

        // 이진값(0/1)에 대한 박스 합으로 정확한 정사각형 팽창 / 침식을 O(n)에 계산
        var bin = new int[w * h];
        for (int i = 0; i < bin.Length; i++) bin[i] = src.Data[i] >= 128 ? 1 : 0;
        var horiz = BoxSum(bin, w, h, r, horizontal: true);
        var sums = BoxSum(horiz, w, h, r, horizontal: false);

        var dst = new Mask(w, h);
        Parallel.For(0, h, y =>
        {
            int y0 = Math.Max(0, y - r), y1 = Math.Min(h - 1, y + r);
            for (int x = 0; x < w; x++)
            {
                int i = y * w + x;
                bool on;
                if (dilate)
                {
                    on = sums[i] > 0;
                }
                else
                {
                    int x0 = Math.Max(0, x - r), x1 = Math.Min(w - 1, x + r);
                    on = sums[i] == (x1 - x0 + 1) * (y1 - y0 + 1);
                }
                dst.Data[i] = on ? (byte)255 : (byte)0;
            }
        });
        return dst;
    }

    private static int[] BoxSum(int[] src, int w, int h, int r, bool horizontal)
    {
        var dst = new int[src.Length];
        int lines = horizontal ? h : w;
        int len = horizontal ? w : h;
        int step = horizontal ? 1 : w;
        Parallel.For(0, lines, line =>
        {
            int start = horizontal ? line * w : line;
            int sum = 0;
            for (int k = 0; k <= Math.Min(r, len - 1); k++) sum += src[start + k * step];
            for (int k = 0; k < len; k++)
            {
                dst[start + k * step] = sum;
                int add = k + r + 1;
                int sub = k - r;
                if (add < len) sum += src[start + add * step];
                if (sub >= 0) sum -= src[start + sub * step];
            }
        });
        return dst;
    }

    /// <summary>박스 블러 2회 (가우시안 근사)로 경계를 부드럽게 한다.</summary>
    public static Mask Feather(Mask src, int radius)
    {
        if (radius <= 0) return src;
        int w = src.Width, h = src.Height;
        var buf = new int[w * h];
        for (int i = 0; i < buf.Length; i++) buf[i] = src.Data[i];

        for (int pass = 0; pass < 2; pass++)
        {
            buf = BoxAverage(buf, w, h, radius, horizontal: true);
            buf = BoxAverage(buf, w, h, radius, horizontal: false);
        }

        var dst = new Mask(w, h);
        for (int i = 0; i < buf.Length; i++) dst.Data[i] = (byte)Math.Clamp(buf[i], 0, 255);
        return dst;
    }

    private static int[] BoxAverage(int[] src, int w, int h, int r, bool horizontal)
    {
        var dst = new int[src.Length];
        int lines = horizontal ? h : w;
        int len = horizontal ? w : h;
        int step = horizontal ? 1 : w;
        int window = 2 * r + 1;
        Parallel.For(0, lines, line =>
        {
            int start = horizontal ? line * w : line;
            // 가장자리는 끝 픽셀을 반복한 것으로 취급
            int Get(int k) => src[start + Math.Clamp(k, 0, len - 1) * step];
            int sum = 0;
            for (int k = -r; k <= r; k++) sum += Get(k);
            for (int k = 0; k < len; k++)
            {
                dst[start + k * step] = (sum + window / 2) / window;
                sum += Get(k + r + 1) - Get(k - r);
            }
        });
        return dst;
    }

    /// <summary>여러 마스크의 픽셀별 최댓값 (합집합).</summary>
    public static Mask Union(int width, int height, IEnumerable<Mask> masks)
    {
        var dst = new Mask(width, height);
        foreach (var m in masks)
        {
            var a = dst.Data;
            var b = m.Data;
            for (int i = 0; i < a.Length; i++)
            {
                if (b[i] > a[i]) a[i] = b[i];
            }
        }
        return dst;
    }
}
