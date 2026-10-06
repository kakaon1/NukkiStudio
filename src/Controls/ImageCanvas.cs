using System.ComponentModel;
using System.Drawing.Drawing2D;
using NukkiStudio.App.Segmentation;
using NukkiStudio.App.UI;

namespace NukkiStudio.App.Controls;

public enum CanvasTool
{
    Point,
    Box,
    BrushAdd,
    BrushErase,
}

public enum BrushPhase
{
    Begin,
    Move,
    End,
}

public sealed class PointPromptEventArgs(PointF imagePoint, bool positive) : EventArgs
{
    public PointF ImagePoint { get; } = imagePoint;
    public bool Positive { get; } = positive;
}

public sealed class BoxPromptEventArgs(RectangleF imageBox) : EventArgs
{
    public RectangleF ImageBox { get; } = imageBox;
}

public sealed class BrushStrokeEventArgs(BrushPhase phase, PointF from, PointF to, bool add) : EventArgs
{
    public BrushPhase Phase { get; } = phase;
    public PointF From { get; } = from;
    public PointF To { get; } = to;
    public bool Add { get; } = add;
}

/// <summary>
/// 이미지 표시 캔버스: 확대 / 축소 / 이동, 체커보드, 마스크 오버레이, 프롬프트 표시, 마우스 입력.
/// 좌표 변환만 담당하고 마스크 계산은 하지 않는다.
/// </summary>
public sealed class ImageCanvas : Control
{
    private const float MinZoom = 0.02f;
    private const float MaxZoom = 32f;

    private Bitmap? _baseImage;
    private Bitmap? _overlayImage;
    private float _zoom = 1f;
    private PointF _pan;              // 이미지 (0,0)의 화면 좌표
    private bool _spaceDown;

    // 마우스 상태
    private bool _panning;
    private Point _panStartMouse;
    private PointF _panStartOffset;
    private bool _boxDragging;
    private PointF _boxStart;         // 이미지 좌표
    private PointF _boxCurrent;
    private bool _brushing;
    private bool _brushAdd;
    private PointF _lastBrushPoint;
    private Point _mouse = new(-1000, -1000);
    private Point _downMouse;
    private MouseButtons _downButton;

    private TextureBrush? _checker;

    // 축소 표시용 캐시 (원본 / 배율이 바뀌면 다시 만든다)
    private Bitmap? _scaledBase;
    private Bitmap? _scaledSource;
    private int _scaledLevel;

    public ImageCanvas()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        TabStop = true;
    }

    public event EventHandler<PointPromptEventArgs>? PointPrompt;
    public event EventHandler<BoxPromptEventArgs>? BoxPrompt;
    public event EventHandler<BrushStrokeEventArgs>? BrushStroke;
    public event EventHandler? ZoomChanged;

    /// <summary>이미지가 없을 때 빈 화면(드롭 영역)을 클릭하면 발생 — 파일 선택 창을 연다.</summary>
    public event EventHandler? EmptyAreaClick;

    /// <summary>떠 있는 붙여넣기 이미지를 더블클릭하면 발생 — 확정.</summary>
    public event EventHandler? FloatingCommit;

    // ---------------- 붙여넣기 (떠 있는 이미지) ----------------

    private Bitmap? _floatingImage;
    private bool _floatDragging;
    private PointF _floatGrab;

    /// <summary>Ctrl+V로 붙여넣어 아직 확정하지 않은 이미지. 드래그로 이동, Ctrl+휠로 크기 조절.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Bitmap? FloatingImage
    {
        get => _floatingImage;
        set { _floatingImage = value; _floatDragging = false; Invalidate(); }
    }

    /// <summary>떠 있는 이미지의 위치 / 크기 (원본 이미지 좌표).</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RectangleF FloatingRect { get; set; }

    private bool HasFloating => _floatingImage is not null && _baseImage is not null;

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Bitmap? BaseImage
    {
        get => _baseImage;
        set
        {
            _baseImage = value;
            // 빈 화면은 클릭하면 파일을 열 수 있다는 표시
            if (!_panning) Cursor = value is null ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }
    }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Bitmap? OverlayImage
    {
        get => _overlayImage;
        set { _overlayImage = value; Invalidate(); }
    }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowCheckerboard { get; set; }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CanvasTool Tool { get; set; } = CanvasTool.Point;

    /// <summary>브러시 반지름 (이미지 픽셀).</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float BrushRadius { get; set; } = 15f;

    /// <summary>true면 네모 브러시, false면 원형 브러시.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool BrushSquare { get; set; }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<PromptPoint> Points { get; set; } = Array.Empty<PromptPoint>();

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RectangleF? Box { get; set; }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HintText { get; set; } = "이미지 파일이나 폴더를 이곳에 끌어다 놓으세요";

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HintSubText { get; set; } = "또는 클릭해서 파일 선택  ·  Ctrl+V로 붙여넣기";

    [Browsable(false)]
    public float Zoom => _zoom;

    // ---------------- 좌표 / 확대 ----------------

    public PointF ScreenToImage(Point p) => new((p.X - _pan.X) / _zoom, (p.Y - _pan.Y) / _zoom);

    private PointF ImageToScreen(PointF p) => new(p.X * _zoom + _pan.X, p.Y * _zoom + _pan.Y);

    private RectangleF ImageRectOnScreen() =>
        _baseImage is null ? RectangleF.Empty : new RectangleF(_pan.X, _pan.Y, _baseImage.Width * _zoom, _baseImage.Height * _zoom);

    public void FitToWindow()
    {
        if (_baseImage is null || ClientSize.Width <= 0 || ClientSize.Height <= 0) return;
        const int margin = 16;
        float zx = (ClientSize.Width - margin * 2f) / _baseImage.Width;
        float zy = (ClientSize.Height - margin * 2f) / _baseImage.Height;
        _zoom = Math.Clamp(Math.Min(zx, zy), MinZoom, MaxZoom);
        CenterImage();
    }

    public void ActualSize()
    {
        if (_baseImage is null) return;
        _zoom = 1f;
        CenterImage();
    }

    private void CenterImage()
    {
        if (_baseImage is null) return;
        _pan = new PointF((ClientSize.Width - _baseImage.Width * _zoom) / 2f, (ClientSize.Height - _baseImage.Height * _zoom) / 2f);
        Invalidate();
        ZoomChanged?.Invoke(this, EventArgs.Empty);
    }

    // ---------------- 스크롤바 (확대해서 이미지가 화면보다 클 때만 표시) ----------------

    private const int BarSize = 12;
    private const int MinThumb = 30;

    private enum ScrollAxis { None, Horizontal, Vertical }

    private ScrollAxis _scrollDrag;
    private ScrollAxis _scrollHover;
    private Point _scrollStartMouse;
    private PointF _scrollStartPan;

    /// <summary>스크롤 범위: 이미지 영역과 화면 영역을 합친 구간 (화면 좌표).</summary>
    private (float Start, float Length) ScrollRange(bool horizontal)
    {
        var dest = ImageRectOnScreen();
        float view = horizontal ? Width : Height;
        float a = horizontal ? dest.Left : dest.Top;
        float b = horizontal ? dest.Right : dest.Bottom;
        float start = Math.Min(a, 0), end = Math.Max(b, view);
        return (start, end - start);
    }

    private bool ShowHorizontalBar => _baseImage is not null && ImageRectOnScreen().Width > Width + 0.5f;

    private bool ShowVerticalBar => _baseImage is not null && ImageRectOnScreen().Height > Height + 0.5f;

    private Rectangle TrackRect(ScrollAxis axis) => axis == ScrollAxis.Horizontal
        ? new Rectangle(0, Height - BarSize, Width - (ShowVerticalBar ? BarSize : 0), BarSize)
        : new Rectangle(Width - BarSize, 0, BarSize, Height - (ShowHorizontalBar ? BarSize : 0));

    /// <summary>스크롤 막대 손잡이 위치 (트랙 안).</summary>
    private RectangleF ThumbRect(ScrollAxis axis)
    {
        bool horizontal = axis == ScrollAxis.Horizontal;
        var track = TrackRect(axis);
        float trackLength = horizontal ? track.Width : track.Height;
        var (start, length) = ScrollRange(horizontal);
        float view = horizontal ? Width : Height;
        float thumb = Math.Max(MinThumb, trackLength * view / length);
        float movable = Math.Max(1, trackLength - thumb);
        float scrollable = Math.Max(1, length - view);
        float pos = (0 - start) / scrollable * movable;
        return horizontal
            ? new RectangleF(track.X + pos, track.Y + 2, thumb, BarSize - 4)
            : new RectangleF(track.X + 2, track.Y + pos, BarSize - 4, thumb);
    }

    private ScrollAxis HitScrollBar(Point p)
    {
        if (ShowHorizontalBar && TrackRect(ScrollAxis.Horizontal).Contains(p)) return ScrollAxis.Horizontal;
        if (ShowVerticalBar && TrackRect(ScrollAxis.Vertical).Contains(p)) return ScrollAxis.Vertical;
        return ScrollAxis.None;
    }

    /// <summary>손잡이를 마우스 이동량만큼 옮긴 것에 맞춰 화면을 이동한다.</summary>
    private void DragScroll(Point mouse)
    {
        bool horizontal = _scrollDrag == ScrollAxis.Horizontal;
        var track = TrackRect(_scrollDrag);
        float trackLength = horizontal ? track.Width : track.Height;
        var (_, length) = ScrollRangeAt(horizontal, _scrollStartPan);
        float view = horizontal ? Width : Height;
        float thumb = Math.Max(MinThumb, trackLength * view / length);
        float movable = Math.Max(1, trackLength - thumb);
        float delta = horizontal ? mouse.X - _scrollStartMouse.X : mouse.Y - _scrollStartMouse.Y;
        float panDelta = -delta * Math.Max(1, length - view) / movable;
        var dest = ImageRectOnScreen();
        if (horizontal)
            _pan = new PointF(ClampPan(_scrollStartPan.X + panDelta, dest.Width, Width), _pan.Y);
        else
            _pan = new PointF(_pan.X, ClampPan(_scrollStartPan.Y + panDelta, dest.Height, Height));
        Invalidate();
    }

    private (float Start, float Length) ScrollRangeAt(bool horizontal, PointF pan)
    {
        var saved = _pan;
        _pan = pan;
        var range = ScrollRange(horizontal);
        _pan = saved;
        return range;
    }

    /// <summary>스크롤로 이동할 때 이미지 가장자리가 화면 안쪽으로 들어오지 않게 제한.</summary>
    private static float ClampPan(float pan, float content, float view) =>
        content <= view ? pan : Math.Clamp(pan, view - content, 0);

    /// <summary>트랙의 빈 곳을 클릭하면 그 위치가 손잡이 가운데가 되도록 이동.</summary>
    private void JumpScroll(ScrollAxis axis, Point mouse)
    {
        bool horizontal = axis == ScrollAxis.Horizontal;
        var track = TrackRect(axis);
        var thumb = ThumbRect(axis);
        float trackLength = horizontal ? track.Width : track.Height;
        float thumbLength = horizontal ? thumb.Width : thumb.Height;
        float movable = Math.Max(1, trackLength - thumbLength);
        float target = Math.Clamp((horizontal ? mouse.X - track.X : mouse.Y - track.Y) - thumbLength / 2, 0, movable);
        var dest = ImageRectOnScreen();
        float content = horizontal ? dest.Width : dest.Height;
        float view = horizontal ? Width : Height;
        float pan = -(target / movable) * (content - view);
        _pan = horizontal ? new PointF(ClampPan(pan, content, view), _pan.Y) : new PointF(_pan.X, ClampPan(pan, content, view));
        Invalidate();
    }

    private void DrawScrollBars(Graphics g)
    {
        foreach (var axis in new[] { ScrollAxis.Horizontal, ScrollAxis.Vertical })
        {
            if (axis == ScrollAxis.Horizontal ? !ShowHorizontalBar : !ShowVerticalBar) continue;
            using (var track = new SolidBrush(Color.FromArgb(150, Theme.Canvas))) g.FillRectangle(track, TrackRect(axis));
            bool active = _scrollDrag == axis || _scrollHover == axis;
            using var path = Theme.RoundRect(ThumbRect(axis), 4);
            using var thumb = new SolidBrush(active ? Theme.AccentHover : Color.FromArgb(200, Theme.RaisedHover));
            g.FillPath(thumb, path);
        }
    }

    private void ZoomAt(Point screen, float newZoom)
    {
        newZoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
        var img = ScreenToImage(screen);
        _zoom = newZoom;
        _pan = new PointF(screen.X - img.X * _zoom, screen.Y - img.Y * _zoom);
        Invalidate();
        ZoomChanged?.Invoke(this, EventArgs.Empty);
    }

    // ---------------- 그리기 ----------------

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(BackColor);

        if (_baseImage is null)
        {
            DrawEmptyState(g);
            return;
        }

        var dest = ImageRectOnScreen();
        DrawShadow(g, dest);
        if (ShowCheckerboard)
        {
            _checker ??= CreateCheckerBrush();
            g.FillRectangle(_checker, dest);
        }

        try
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            if (_zoom < 0.5f)
            {
                // 크게 축소: 1/2, 1/4 ... 크기로 고품질 축소해 둔 사본을 부드럽게 그린다 (계단 / 깨짐 방지, 휠마다 다시 만들지 않음)
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                DrawVisible(g, GetReducedBase(), dest);
            }
            else
            {
                // 1/2~3배: 부드럽게, 3배 이상: 픽셀 단위 작업을 위해 픽셀 그대로
                g.InterpolationMode = _zoom >= 3f ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBilinear;
                DrawVisible(g, _baseImage, dest);
            }
            if (_overlayImage is not null) DrawVisible(g, _overlayImage, dest);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            // 이미지가 막 교체되는 중 (잠김 / 해제됨) — 이번 그리기는 건너뛰고 다시 그린다. 빨간 X 오류 화면 방지.
            BeginInvoke(new Action(Invalidate));
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.Default;
        DrawPrompts(g);
        DrawToolFeedback(g);
        DrawFloating(g);
        DrawScrollBars(g);
    }

    private void DrawFloating(Graphics g)
    {
        if (!HasFloating) return;
        var a = ImageToScreen(FloatingRect.Location);
        var b = ImageToScreen(new PointF(FloatingRect.Right, FloatingRect.Bottom));
        var r = RectangleF.FromLTRB(a.X, a.Y, b.X, b.Y);
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        g.DrawImage(_floatingImage!, r);
        using var pen = new Pen(Theme.AccentHover, 1.5f) { DashStyle = DashStyle.Dash };
        g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);

        const string hint = "드래그: 이동  ·  Ctrl+휠: 크기  ·  Enter / 더블클릭: 붙여넣기  ·  Esc: 취소";
        var size = TextRenderer.MeasureText(hint, Font);
        var box = new Rectangle((Width - size.Width) / 2 - 12, 12, size.Width + 24, size.Height + 12);
        using (var path = Theme.RoundRect(box, 8))
        using (var fill = new SolidBrush(Color.FromArgb(230, Theme.Card)))
            g.FillPath(fill, path);
        TextRenderer.DrawText(g, hint, Font, box, Theme.Text, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    /// <summary>이미지가 없을 때: 점선 드롭 영역 + 아이콘 + 안내 문구.</summary>
    private void DrawEmptyState(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var zone = new RectangleF((Width - 440) / 2f, (Height - 240) / 2f, 440, 240);
        if (zone.Width > Width - 40) zone = new RectangleF(20, zone.Y, Width - 40, zone.Height);

        using (var path = Theme.RoundRect(zone, 16))
        {
            using var fill = new SolidBrush(Color.FromArgb(22, 22, 25));
            g.FillPath(fill, path);
            using var pen = new Pen(Theme.Border, 1.5f) { DashPattern = new[] { 5f, 4f } };
            g.DrawPath(pen, path);
        }

        // 아이콘 원
        float cx = zone.X + zone.Width / 2;
        var circle = new RectangleF(cx - 30, zone.Y + 34, 60, 60);
        using (var bg = new SolidBrush(Color.FromArgb(40, Theme.Accent))) g.FillEllipse(bg, circle);
        var state = g.Save();
        g.TranslateTransform(circle.X + 15, circle.Y + 15);
        g.ScaleTransform(30 / 24f, 30 / 24f);
        IconFactory.Draw(g, AppIcon.Image, Theme.AccentHover);
        g.Restore(state);

        using var titleFont = new Font(Font.FontFamily, 12f, FontStyle.Bold);
        var titleRect = new Rectangle((int)zone.X, (int)circle.Bottom + 18, (int)zone.Width, 26);
        TextRenderer.DrawText(g, HintText, titleFont, titleRect, Theme.Text, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        var subRect = new Rectangle((int)zone.X, titleRect.Bottom + 4, (int)zone.Width, 22);
        TextRenderer.DrawText(g, HintSubText, Font, subRect, Theme.TextDim, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private static void DrawShadow(Graphics g, RectangleF dest)
    {
        // 이미지 뒤 은은한 그림자 (바깥으로 갈수록 옅게)
        for (int i = 1; i <= 6; i++)
        {
            using var pen = new Pen(Color.FromArgb(28 - i * 4, 0, 0, 0), 2f);
            g.DrawRectangle(pen, dest.X - i * 1.5f, dest.Y - i * 1.5f + 2, dest.Width + i * 3f, dest.Height + i * 3f);
        }
    }

    /// <summary>
    /// 현재 배율 이하에서 가장 가까운 1/2^k 크기로 고품질(HighQualityBicubic) 축소한 원본.
    /// 같은 원본 / 같은 단계면 재사용하므로 휠로 확대·축소해도 매번 다시 만들지 않는다.
    /// </summary>
    private Bitmap GetReducedBase()
    {
        var src = _baseImage!;
        int level = Math.Max(1, (int)Math.Floor(Math.Log2(1.0 / _zoom)));
        if (_scaledBase is not null && ReferenceEquals(_scaledSource, src) && _scaledLevel == level) return _scaledBase;

        int w = Math.Max(1, src.Width >> level);
        int h = Math.Max(1, src.Height >> level);
        var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using (var g = Graphics.FromImage(bmp))
        using (var attributes = new System.Drawing.Imaging.ImageAttributes())
        {
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            attributes.SetWrapMode(WrapMode.TileFlipXY); // 가장자리 번짐 방지
            g.DrawImage(src, new Rectangle(0, 0, w, h), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, attributes);
        }
        _scaledBase?.Dispose();
        _scaledBase = bmp;
        _scaledSource = src;
        _scaledLevel = level;
        return bmp;
    }

    /// <summary>화면에 보이는 부분만 잘라서 그린다 (큰 이미지 확대 시 속도).</summary>
    private void DrawVisible(Graphics g, Bitmap bmp, RectangleF dest)
    {
        var visible = RectangleF.Intersect(dest, ClientRectangle);
        if (visible.Width <= 0 || visible.Height <= 0) return;
        // 축소 사본이면 원본 대비 비율만큼 원본 좌표를 줄인다
        float ratio = _baseImage is null ? 1f : (float)bmp.Width / _baseImage.Width;
        var src = new RectangleF((visible.X - _pan.X) / _zoom * ratio, (visible.Y - _pan.Y) / _zoom * ratio, visible.Width / _zoom * ratio, visible.Height / _zoom * ratio);
        g.DrawImage(bmp, visible, src, GraphicsUnit.Pixel);
    }

    private void DrawPrompts(Graphics g)
    {
        if (Box is { } box)
        {
            var a = ImageToScreen(box.Location);
            var b = ImageToScreen(new PointF(box.Right, box.Bottom));
            using var pen = new Pen(Color.Yellow, 2f) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, a.X, a.Y, b.X - a.X, b.Y - a.Y);
        }

        foreach (var p in Points)
        {
            var s = ImageToScreen(new PointF(p.X, p.Y));
            const float r = 7f;
            using var fill = new SolidBrush(p.Positive ? Color.FromArgb(240, Theme.Success) : Color.FromArgb(240, Theme.Danger));
            g.FillEllipse(fill, s.X - r, s.Y - r, r * 2, r * 2);
            g.DrawEllipse(Pens.White, s.X - r, s.Y - r, r * 2, r * 2);
            using var mark = new Pen(Color.White, 2f);
            g.DrawLine(mark, s.X - 4, s.Y, s.X + 4, s.Y);
            if (p.Positive) g.DrawLine(mark, s.X, s.Y - 4, s.X, s.Y + 4);
        }
    }

    private void DrawToolFeedback(Graphics g)
    {
        if (_boxDragging)
        {
            var a = ImageToScreen(_boxStart);
            var b = ImageToScreen(_boxCurrent);
            var r = RectangleF.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
            using var pen = new Pen(Color.Cyan, 1.5f) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
        }

        if ((Tool == CanvasTool.BrushAdd || Tool == CanvasTool.BrushErase) && ClientRectangle.Contains(_mouse) && !_panning && _scrollHover == ScrollAxis.None)
        {
            float r = BrushRadius * _zoom;
            bool add = _brushing ? _brushAdd : Tool == CanvasTool.BrushAdd;
            using var pen = new Pen(add ? Color.LimeGreen : Color.OrangeRed, 1.5f);
            if (BrushSquare)
            {
                g.DrawRectangle(Pens.Black, _mouse.X - r - 1, _mouse.Y - r - 1, r * 2 + 2, r * 2 + 2);
                g.DrawRectangle(pen, _mouse.X - r, _mouse.Y - r, r * 2, r * 2);
            }
            else
            {
                g.DrawEllipse(Pens.Black, _mouse.X - r - 1, _mouse.Y - r - 1, r * 2 + 2, r * 2 + 2);
                g.DrawEllipse(pen, _mouse.X - r, _mouse.Y - r, r * 2, r * 2);
            }
        }
    }

    private static TextureBrush CreateCheckerBrush()
    {
        const int s = 10;
        var bmp = new Bitmap(s * 2, s * 2);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.FromArgb(58, 58, 64));
            using var dark = new SolidBrush(Color.FromArgb(44, 44, 49));
            g.FillRectangle(dark, 0, 0, s, s);
            g.FillRectangle(dark, s, s, s, s);
        }
        return new TextureBrush(bmp);
    }

    // ---------------- 마우스 ----------------

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        if (FindForm()?.ContainsFocus == true && !Focused) Focus();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
        _downMouse = e.Location;
        _downButton = e.Button;
        if (_baseImage is null) return;

        // 스크롤 막대: 손잡이는 드래그, 빈 트랙은 그 위치로 이동 후 드래그
        var bar = HitScrollBar(e.Location);
        if (bar != ScrollAxis.None)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!ThumbRect(bar).Contains(e.Location)) JumpScroll(bar, e.Location);
                _scrollDrag = bar;
                _scrollStartMouse = e.Location;
                _scrollStartPan = _pan;
            }
            return;
        }

        bool panGesture = e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && _spaceDown);
        if (panGesture)
        {
            _panning = true;
            _panStartMouse = e.Location;
            _panStartOffset = _pan;
            Cursor = Cursors.SizeAll;
            return;
        }

        var img = ScreenToImage(e.Location);
        if (HasFloating)
        {
            // 붙여넣기 중에는 떠 있는 이미지 이동만
            if (e.Button == MouseButtons.Left)
            {
                _floatDragging = true;
                _floatGrab = new PointF(img.X - FloatingRect.X, img.Y - FloatingRect.Y);
                Cursor = Cursors.SizeAll;
            }
            return;
        }
        switch (Tool)
        {
            case CanvasTool.Box when e.Button == MouseButtons.Left:
                _boxDragging = true;
                _boxStart = _boxCurrent = img;
                break;
            case CanvasTool.BrushAdd or CanvasTool.BrushErase when e.Button is MouseButtons.Left or MouseButtons.Right:
                // 우클릭은 반대 동작
                _brushing = true;
                _brushAdd = (Tool == CanvasTool.BrushAdd) == (e.Button == MouseButtons.Left);
                _lastBrushPoint = img;
                BrushStroke?.Invoke(this, new BrushStrokeEventArgs(BrushPhase.Begin, img, img, _brushAdd));
                break;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var old = _mouse;
        _mouse = e.Location;

        if (_scrollDrag != ScrollAxis.None)
        {
            DragScroll(e.Location);
            return;
        }

        // 스크롤 막대 위에서는 손잡이 강조 + 기본 커서 (작업 도구가 동작하지 않음)
        var hover = _panning || _brushing || _boxDragging || _floatDragging ? ScrollAxis.None : HitScrollBar(e.Location);
        if (hover != _scrollHover)
        {
            _scrollHover = hover;
            Invalidate();
        }

        if (_panning)
        {
            _pan = new PointF(_panStartOffset.X + e.X - _panStartMouse.X, _panStartOffset.Y + e.Y - _panStartMouse.Y);
            Invalidate();
            return;
        }

        if (_floatDragging)
        {
            var img = ScreenToImage(e.Location);
            FloatingRect = new RectangleF(img.X - _floatGrab.X, img.Y - _floatGrab.Y, FloatingRect.Width, FloatingRect.Height);
            Invalidate();
            return;
        }

        if (_boxDragging)
        {
            _boxCurrent = ScreenToImage(e.Location);
            Invalidate();
            return;
        }

        if (_brushing)
        {
            var img = ScreenToImage(e.Location);
            BrushStroke?.Invoke(this, new BrushStrokeEventArgs(BrushPhase.Move, _lastBrushPoint, img, _brushAdd));
            _lastBrushPoint = img;
        }

        if (Tool is CanvasTool.BrushAdd or CanvasTool.BrushErase)
        {
            // 브러시 커서 영역만 다시 그리기
            int r = (int)(BrushRadius * _zoom) + 4;
            Invalidate(new Rectangle(old.X - r, old.Y - r, r * 2, r * 2));
            Invalidate(new Rectangle(_mouse.X - r, _mouse.Y - r, r * 2, r * 2));
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_scrollDrag != ScrollAxis.None)
        {
            _scrollDrag = ScrollAxis.None;
            Invalidate();
            return;
        }
        // 스크롤 막대 위에서 누른 클릭은 작업 도구로 처리하지 않는다
        if (HitScrollBar(_downMouse) != ScrollAxis.None && !_brushing && !_boxDragging && !_panning && !_floatDragging) return;

        if (_panning)
        {
            _panning = false;
            Cursor = Cursors.Default;
            return;
        }

        if (_floatDragging)
        {
            _floatDragging = false;
            Cursor = Cursors.Default;
            return;
        }
        if (HasFloating) return;

        bool isClick = Math.Abs(e.X - _downMouse.X) <= 3 && Math.Abs(e.Y - _downMouse.Y) <= 3;
        if (_baseImage is null)
        {
            if (isClick && e.Button == MouseButtons.Left) EmptyAreaClick?.Invoke(this, EventArgs.Empty);
            return;
        }
        var img = ScreenToImage(e.Location);

        if (_boxDragging)
        {
            _boxDragging = false;
            var r = RectangleF.FromLTRB(Math.Min(_boxStart.X, img.X), Math.Min(_boxStart.Y, img.Y), Math.Max(_boxStart.X, img.X), Math.Max(_boxStart.Y, img.Y));
            r.Intersect(new RectangleF(0, 0, _baseImage.Width, _baseImage.Height));
            Invalidate();
            if (!isClick && r.Width >= 2 && r.Height >= 2)
            {
                BoxPrompt?.Invoke(this, new BoxPromptEventArgs(r));
            }
            else if (InsideImage(img))
            {
                // 박스 도구에서 그냥 클릭하면 포함 점
                PointPrompt?.Invoke(this, new PointPromptEventArgs(img, true));
            }
            return;
        }

        if (_brushing)
        {
            _brushing = false;
            BrushStroke?.Invoke(this, new BrushStrokeEventArgs(BrushPhase.End, _lastBrushPoint, img, _brushAdd));
            return;
        }

        if (isClick && InsideImage(img) && Tool is CanvasTool.Point or CanvasTool.Box && e.Button == _downButton)
        {
            if (e.Button == MouseButtons.Left) PointPrompt?.Invoke(this, new PointPromptEventArgs(img, true));
            else if (e.Button == MouseButtons.Right) PointPrompt?.Invoke(this, new PointPromptEventArgs(img, false));
        }
    }

    private bool InsideImage(PointF img) =>
        _baseImage is not null && img.X >= 0 && img.Y >= 0 && img.X < _baseImage.Width && img.Y < _baseImage.Height;

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _mouse = new Point(-1000, -1000);
        _scrollHover = ScrollAxis.None;
        Invalidate();
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (HasFloating && e.Button == MouseButtons.Left) FloatingCommit?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_baseImage is null) return;
        if (HasFloating && (ModifierKeys & Keys.Control) != 0)
        {
            // 떠 있는 이미지 크기 조절 (가운데 기준)
            float s = e.Delta > 0 ? 1.1f : 1f / 1.1f;
            var r = FloatingRect;
            float w = Math.Max(4, r.Width * s), h = Math.Max(4, r.Height * s);
            FloatingRect = new RectangleF(r.X + (r.Width - w) / 2, r.Y + (r.Height - h) / 2, w, h);
            Invalidate();
            return;
        }
        if ((ModifierKeys & Keys.Shift) != 0)
        {
            // Shift+휠: 좌우 이동
            var dest = ImageRectOnScreen();
            _pan = new PointF(ClampPan(_pan.X + e.Delta * 0.5f, dest.Width, Width), _pan.Y);
            Invalidate();
            return;
        }
        float factor = e.Delta > 0 ? 1.2f : 1f / 1.2f;
        ZoomAt(e.Location, _zoom * factor);
    }

    // ---------------- 키보드 ----------------

    protected override bool IsInputKey(Keys keyData) => keyData == Keys.Space || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Space && !_spaceDown)
        {
            _spaceDown = true;
            Cursor = Cursors.Hand;
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.KeyCode == Keys.Space)
        {
            _spaceDown = false;
            if (!_panning) Cursor = Cursors.Default;
            e.Handled = true;
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        _spaceDown = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _checker?.Dispose();
            _scaledBase?.Dispose();
        }
        base.Dispose(disposing);
    }
}
