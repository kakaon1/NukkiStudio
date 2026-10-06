using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Inpainting;

/// <summary>
/// AI 모델이 없을 때 쓰는 기본 채우기: 경계에서 안쪽으로 한 겹씩 주변 평균색으로 채운 뒤 몇 번 부드럽게 한다.
/// 작은 객체나 단순한 배경에 적합하다 (큰 영역은 흐릿해진다).
/// </summary>
public sealed class DiffusionInpainter : IInpainter
{
    public string Name => "기본 채우기";

    public ImageBuffer Inpaint(ImageBuffer image, Mask hole)
    {
        int w = image.Width, h = image.Height;
        var holeMask = MaskOps.Grow(CropInpainter.BinaryOf(hole), 2);
        var px = (byte[])image.Pixels.Clone();
        var filled = new bool[w * h];
        var queue = new Queue<int>();
        var order = new List<int>();

        for (int i = 0; i < filled.Length; i++) filled[i] = holeMask.Data[i] < 128;

        // 경계(채워진 이웃이 있는 구멍 픽셀)부터 BFS 순서 계산
        var queued = new bool[w * h];
        for (int i = 0; i < filled.Length; i++)
        {
            if (filled[i]) continue;
            if (HasFilledNeighbor(i)) { queue.Enqueue(i); queued[i] = true; }
        }
        while (queue.Count > 0)
        {
            int p = queue.Dequeue();
            order.Add(p);
            int x = p % w, y = p / w;
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if ((dx | dy) == 0 || nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                int q = ny * w + nx;
                if (filled[q] || queued[q]) continue;
                queued[q] = true;
                queue.Enqueue(q);
            }
        }

        // 순서대로: 이미 채워진 이웃의 평균
        foreach (int p in order)
        {
            AverageNeighbors(p, onlyFilled: true);
            filled[p] = true;
        }

        // 부드럽게 (구멍 내부만)
        for (int pass = 0; pass < 4; pass++)
        {
            foreach (int p in order) AverageNeighbors(p, onlyFilled: false);
        }

        return new ImageBuffer(w, h, px);

        bool HasFilledNeighbor(int p)
        {
            int x = p % w, y = p / w;
            return (x > 0 && filled[p - 1]) || (x < w - 1 && filled[p + 1]) || (y > 0 && filled[p - w]) || (y < h - 1 && filled[p + w]);
        }

        void AverageNeighbors(int p, bool onlyFilled)
        {
            int x = p % w, y = p / w;
            int sb = 0, sg = 0, sr = 0, sa = 0, n = 0;
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if ((dx | dy) == 0 || nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                int q = ny * w + nx;
                if (onlyFilled && !filled[q]) continue;
                int qi = q * 4;
                sb += px[qi]; sg += px[qi + 1]; sr += px[qi + 2]; sa += px[qi + 3];
                n++;
            }
            if (n == 0) return;
            int pi = p * 4;
            px[pi] = (byte)(sb / n);
            px[pi + 1] = (byte)(sg / n);
            px[pi + 2] = (byte)(sr / n);
            px[pi + 3] = (byte)(sa / n);
        }
    }

    public void Dispose()
    {
    }
}
