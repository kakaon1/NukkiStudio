using System.Drawing;

namespace NukkiStudio.App.Imaging;

/// <summary>
/// 픽셀당 1바이트 마스크. 0 = 배경, 255 = 객체, 중간값 = 부드러운 경계.
/// 한 번 만들어진 마스크는 수정하지 않고 새 인스턴스를 만든다 (실행 취소 스냅샷 공유를 위해).
/// </summary>
public sealed class Mask
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Data { get; }

    public Mask(int width, int height)
        : this(width, height, new byte[checked(width * height)])
    {
    }

    public Mask(int width, int height, byte[] data)
    {
        if (data.Length != width * height) throw new ArgumentException("마스크 버퍼 크기가 맞지 않습니다.", nameof(data));
        Width = width;
        Height = height;
        Data = data;
    }

    public Mask Clone() => new(Width, Height, (byte[])Data.Clone());

    public bool IsEmpty()
    {
        return Data.AsSpan().IndexOfAnyExcept((byte)0) < 0;
    }

    /// <summary>값이 threshold 이상인 픽셀의 경계 사각형. 비어 있으면 Rectangle.Empty.</summary>
    public Rectangle GetBounds(byte threshold = 1)
    {
        int minX = Width, minY = Height, maxX = -1, maxY = -1;
        for (int y = 0; y < Height; y++)
        {
            var row = Data.AsSpan(y * Width, Width);
            int first = -1;
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] >= threshold) { first = x; break; }
            }
            if (first < 0) continue;
            int last = first;
            for (int x = row.Length - 1; x > first; x--)
            {
                if (row[x] >= threshold) { last = x; break; }
            }
            if (first < minX) minX = first;
            if (last > maxX) maxX = last;
            if (minY == Height) minY = y;
            maxY = y;
        }
        return maxX < 0 ? Rectangle.Empty : Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
    }
}
