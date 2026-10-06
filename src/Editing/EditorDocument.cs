using System.Drawing;
using NukkiStudio.App.Imaging;
using NukkiStudio.App.Segmentation;

namespace NukkiStudio.App.Editing;

/// <summary>마스크 보정 설정.</summary>
public readonly record struct RefineSettings(bool Cleanup, int Grow, int Feather);

/// <summary>
/// 열린 이미지 하나의 편집 상태.
/// - 현재 선택: 프롬프트(점/박스) → AI 마스크 + 브러시 레이어
/// - 확정된 객체 목록
/// - 실행 취소 / 다시 실행
/// </summary>
public sealed class EditorDocument
{
    private const int MaxUndo = 30;

    private static readonly Color[] Palette =
    {
        Color.FromArgb(255, 99, 71), Color.FromArgb(50, 205, 50), Color.FromArgb(255, 165, 0),
        Color.FromArgb(186, 85, 211), Color.FromArgb(0, 206, 209), Color.FromArgb(255, 215, 0),
        Color.FromArgb(255, 105, 180), Color.FromArgb(124, 252, 0),
    };

    private readonly Stack<Snapshot> _undo = new();
    private readonly Stack<Snapshot> _redo = new();

    private List<PromptPoint> _points = new();
    private int _objectCounter;

    /// <summary>현재 이미지. 객체 지우기 / 배경 지우기로 바뀌며 실행 취소에 포함된다.</summary>
    public ImageBuffer Image { get; private set; }

    /// <summary>원본 이미지 대비 편집(지우기)이 적용되었는지.</summary>
    public bool IsImageEdited { get; private set; }
    public string? SourcePath { get; }
    public List<SegmentedObject> Objects { get; private set; } = new();

    public IReadOnlyList<PromptPoint> Points => _points;
    public RectangleF? Box { get; private set; }

    /// <summary>디코더가 만든 이진 마스크 (프롬프트가 없으면 null).</summary>
    public Mask? AiMask { get; private set; }

    /// <summary>다음 디코더 호출에 넣을 이전 로짓.</summary>
    public float[]? LowResLogits { get; private set; }

    /// <summary>브러시 레이어: +1 강제 포함, -1 강제 제외, 0 AI 마스크 따름. 사용 전에는 null.</summary>
    public sbyte[]? Brush { get; private set; }

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public bool HasSelection => AiMask is not null || (Brush is not null && Array.IndexOf(Brush, (sbyte)1) >= 0);

    public PromptSet Prompts => new(_points.ToArray(), Box);

    public EditorDocument(ImageBuffer image, string? sourcePath)
    {
        Image = image;
        SourcePath = sourcePath;
    }

    // ---------------- 프롬프트 ----------------

    public void AddPoint(PromptPoint point)
    {
        PushUndo();
        _points = new List<PromptPoint>(_points) { point };
    }

    public void SetBox(RectangleF box)
    {
        PushUndo();
        Box = box;
    }

    /// <summary>디코더 결과 반영 (실행 취소 단위는 AddPoint / SetBox에서 이미 기록됨).</summary>
    public void ApplyPrediction(SegmentationResult? result)
    {
        AiMask = result?.Mask;
        LowResLogits = result?.LowResLogits;
    }

    // ---------------- 브러시 ----------------

    /// <summary>브러시 획 시작 시 한 번 호출 (실행 취소 단위).</summary>
    public void BeginBrushStroke()
    {
        PushUndo();
        Brush = Brush is null ? new sbyte[Image.Width * Image.Height] : (sbyte[])Brush.Clone();
    }

    /// <summary>
    /// from → to 선분을 따라 반지름 radius 원(square면 한 변 2×radius 네모)으로 칠한다. 칠한 영역의 경계 사각형을 반환.
    /// </summary>
    public Rectangle PaintBrush(PointF from, PointF to, float radius, bool add, bool square = false)
    {
        if (Brush is null) BeginBrushStroke();
        var brush = Brush!;
        sbyte value = add ? (sbyte)1 : (sbyte)-1;
        int w = Image.Width, h = Image.Height;

        float dx = to.X - from.X, dy = to.Y - from.Y;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        float step = Math.Max(1f, radius / 3f);
        int steps = Math.Max(1, (int)MathF.Ceiling(dist / step));
        float r2 = radius * radius;

        for (int s = 0; s <= steps; s++)
        {
            float t = (float)s / steps;
            float cx = from.X + dx * t, cy = from.Y + dy * t;
            int x0 = Math.Max(0, (int)(cx - radius)), x1 = Math.Min(w - 1, (int)(cx + radius));
            int y0 = Math.Max(0, (int)(cy - radius)), y1 = Math.Min(h - 1, (int)(cy + radius));
            for (int y = y0; y <= y1; y++)
            {
                float ddy = y + 0.5f - cy;
                for (int x = x0; x <= x1; x++)
                {
                    float ddx = x + 0.5f - cx;
                    if (square || ddx * ddx + ddy * ddy <= r2) brush[y * w + x] = value;
                }
            }
        }

        var bounds = RectangleF.FromLTRB(
            Math.Min(from.X, to.X) - radius, Math.Min(from.Y, to.Y) - radius,
            Math.Max(from.X, to.X) + radius, Math.Max(from.Y, to.Y) + radius);
        return Rectangle.Intersect(Rectangle.Round(bounds), new Rectangle(0, 0, w, h));
    }

    // ---------------- 마스크 생성 ----------------

    private Mask? _cleanSource;
    private bool _cleanFlag;
    private Mask? _cleanResult;

    /// <summary>
    /// AI 마스크 정리 결과 (같은 AI 마스크면 캐시 사용). AI 마스크가 없으면 null.
    /// 항상: 포함 점 / 박스와 연결된 조각만 남김 (옆 물체가 섞이는 것 방지).
    /// cleanup이면 추가로 작은 조각 제거 / 구멍 메우기.
    /// </summary>
    public Mask? GetCleanAiMask(bool cleanup)
    {
        if (AiMask is null) return null;
        if (!ReferenceEquals(_cleanSource, AiMask) || _cleanFlag != cleanup)
        {
            var positives = _points.Where(p => p.Positive).Select(p => new PointF(p.X, p.Y));
            var mask = MaskOps.KeepPromptedRegions(AiMask, positives, Box);
            if (cleanup) mask = MaskOps.Cleanup(mask);
            _cleanResult = mask;
            _cleanSource = AiMask;
            _cleanFlag = cleanup;
        }
        return _cleanResult;
    }

    /// <summary>"라벨 N" 형식의 다음 이름. 기존 번호 중 가장 큰 값 + 1 (삭제 후에도 중복 없음).</summary>
    private string NextName(string label, IEnumerable<SegmentedObject> objects)
    {
        int max = 0;
        foreach (var o in objects)
        {
            if (!o.Name.StartsWith(label + " ", StringComparison.Ordinal)) continue;
            if (int.TryParse(o.Name.AsSpan(label.Length + 1), out int n) && n > max) max = n;
        }
        return $"{label} {max + 1}";
    }

    /// <summary>현재 선택의 이진 마스크 (AI 마스크 정리 + 브러시 덮어쓰기). 선택이 없으면 null.</summary>
    public Mask? BuildSelectionMask(bool cleanup)
    {
        if (!HasSelection) return null;
        int w = Image.Width, h = Image.Height;

        Mask baseMask = GetCleanAiMask(cleanup) ?? new Mask(w, h);
        if (Brush is null) return baseMask;

        var data = (byte[])baseMask.Data.Clone();
        var brush = Brush;
        for (int i = 0; i < data.Length; i++)
        {
            if (brush[i] > 0) data[i] = 255;
            else if (brush[i] < 0) data[i] = 0;
        }
        return new Mask(w, h, data);
    }

    /// <summary>현재 선택에 확장/축소와 페더까지 적용한 최종 마스크.</summary>
    public Mask? BuildFinalMask(RefineSettings settings)
    {
        var mask = BuildSelectionMask(settings.Cleanup);
        if (mask is null) return null;
        mask = MaskOps.Grow(mask, settings.Grow);
        mask = MaskOps.Feather(mask, settings.Feather);
        return mask;
    }

    // ---------------- 객체 ----------------

    /// <summary>현재 선택을 객체로 확정하고 선택을 비운다. 선택이 없으면 null.</summary>
    public SegmentedObject? CommitSelection(RefineSettings settings)
    {
        var mask = BuildFinalMask(settings);
        if (mask is null || mask.IsEmpty()) return null;

        PushUndo();
        _objectCounter++;
        var obj = new SegmentedObject(NextName("객체", Objects), mask, Palette[(_objectCounter - 1) % Palette.Length]);
        Objects = new List<SegmentedObject>(Objects) { obj };
        ResetSelectionState();
        return obj;
    }

    /// <summary>자동 감지로 찾은 객체들을 한 번에 추가한다 (실행 취소 한 단계). 이름이 같으면 번호를 붙인다.</summary>
    public IReadOnlyList<SegmentedObject> AddDetectedObjects(IEnumerable<(string Label, Mask Mask)> found)
    {
        var list = found.Where(f => !f.Mask.IsEmpty()).ToList();
        if (list.Count == 0) return Array.Empty<SegmentedObject>();

        PushUndo();
        var added = new List<SegmentedObject>();
        var objects = new List<SegmentedObject>(Objects);
        foreach (var (label, mask) in list)
        {
            _objectCounter++;
            var obj = new SegmentedObject(NextName(label, objects), mask, Palette[(_objectCounter - 1) % Palette.Length]);
            objects.Add(obj);
            added.Add(obj);
        }
        Objects = objects;
        return added;
    }

    public void RemoveObject(SegmentedObject obj)
    {
        if (!Objects.Contains(obj)) return;
        PushUndo();
        Objects = Objects.Where(o => o != obj).ToList();
    }

    /// <summary>
    /// 이미지를 편집 결과로 바꾼다 (실행 취소 단위). 현재 선택은 비우고,
    /// removedObject가 지정되면 그 객체를 목록에서 뺀다 (객체 지우기).
    /// </summary>
    public void ReplaceImage(ImageBuffer image, SegmentedObject? removedObject = null)
    {
        if (image.Width != Image.Width || image.Height != Image.Height)
            throw new ArgumentException("이미지 크기가 같아야 합니다.");
        PushUndo();
        Image = image;
        IsImageEdited = true;
        if (removedObject is not null) Objects = Objects.Where(o => o != removedObject).ToList();
        ResetSelectionState();
    }

    /// <summary>
    /// 이미지 해상도를 바꾼다 (실행 취소 단위). 확정된 객체 마스크도 같은 비율로 맞추고, 진행 중인 선택은 비운다.
    /// </summary>
    public void ResizeImage(int width, int height) => ApplyResize(PrepareResize(width, height));

    /// <summary>크기 조절 결과 (계산과 적용을 나눠, 무거운 계산만 백그라운드에서 한다).</summary>
    public sealed record ResizeResult(ImageBuffer Source, ImageBuffer Image, List<SegmentedObject> Objects);

    /// <summary>새 크기의 이미지 / 객체 마스크를 계산만 한다 (문서는 바꾸지 않음 — 백그라운드 스레드에서 호출 가능).</summary>
    public ResizeResult PrepareResize(int width, int height)
    {
        var source = Image;
        var objects = Objects;
        var resized = ImageResampler.Resize(source, width, height);
        var resizedObjects = objects
            .Select(o => new SegmentedObject(o.Name, ImageResampler.ResizeMask(o.Mask, width, height), o.Color) { Visible = o.Visible })
            .ToList();
        return new ResizeResult(source, resized, resizedObjects);
    }

    /// <summary>계산한 크기 조절 결과를 적용한다 (UI 스레드, 실행 취소 단위). 그 사이 이미지가 바뀌었으면 무시.</summary>
    public bool ApplyResize(ResizeResult result)
    {
        if (!ReferenceEquals(result.Source, Image)) return false;
        if (result.Image.Width == Image.Width && result.Image.Height == Image.Height) return false;
        PushUndo();
        Image = result.Image;
        IsImageEdited = true;
        Objects = result.Objects;
        ResetSelectionState();
        return true;
    }

    /// <summary>keep 마스크 밖을 투명하게 만든 이미지 (배경 지우기). 원본은 바꾸지 않는다.</summary>
    public static ImageBuffer RemoveBackground(ImageBuffer image, Mask keep)
    {
        var dst = new ImageBuffer(image.Width, image.Height, (byte[])image.Pixels.Clone());
        var px = dst.Pixels;
        var m = keep.Data;
        Parallel.For(0, image.Height, y =>
        {
            int o = y * image.Width;
            for (int x = 0; x < image.Width; x++)
            {
                int i = o + x;
                px[i * 4 + 3] = (byte)(px[i * 4 + 3] * m[i] / 255);
            }
        });
        return dst;
    }

    /// <summary>투명한 픽셀이 하나라도 있는지 (배경 지우기 후 체커보드 표시용).</summary>
    public bool HasTransparency()
    {
        var px = Image.Pixels;
        for (int i = 3; i < px.Length; i += 4)
        {
            if (px[i] < 255) return true;
        }
        return false;
    }

    public void ClearSelection()
    {
        if (!HasSelection && _points.Count == 0 && Box is null) return;
        PushUndo();
        ResetSelectionState();
    }

    private void ResetSelectionState()
    {
        _points = new List<PromptPoint>();
        Box = null;
        AiMask = null;
        LowResLogits = null;
        Brush = null;
    }

    // ---------------- 실행 취소 ----------------

    private sealed record Snapshot(ImageBuffer Image, bool Edited, List<PromptPoint> Points, RectangleF? Box, Mask? AiMask, float[]? LowRes, sbyte[]? Brush, List<SegmentedObject> Objects);

    // 이미지 / 마스크 / 목록은 변경 시 새 인스턴스로 교체하므로 참조만 저장한다. 브러시는 획 시작 때 복제된다.
    private Snapshot Capture() => new(Image, IsImageEdited, _points, Box, AiMask, LowResLogits, Brush, Objects);

    private void Restore(Snapshot s)
    {
        Image = s.Image;
        IsImageEdited = s.Edited;
        _points = s.Points;
        Box = s.Box;
        AiMask = s.AiMask;
        LowResLogits = s.LowRes;
        Brush = s.Brush;
        Objects = s.Objects;
    }

    private void PushUndo()
    {
        _undo.Push(Capture());
        _redo.Clear();
        if (_undo.Count > MaxUndo)
        {
            // 가장 오래된 항목 제거
            var keep = _undo.Take(MaxUndo).Reverse().ToList();
            _undo.Clear();
            foreach (var s in keep) _undo.Push(s);
        }
    }

    public bool Undo()
    {
        if (_undo.Count == 0) return false;
        _redo.Push(Capture());
        Restore(_undo.Pop());
        return true;
    }

    public bool Redo()
    {
        if (_redo.Count == 0) return false;
        _undo.Push(Capture());
        Restore(_redo.Pop());
        return true;
    }
}
