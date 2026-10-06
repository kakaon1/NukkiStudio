using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using NukkiStudio.App.UI;

namespace NukkiStudio.App.Controls;

/// <summary>
/// 브러시 크기 조절용 작은 위(+) / 아래(−) 버튼. 누르고 있으면 계속 바뀐다.
/// </summary>
public sealed class BrushSizeStepper : Control
{
    private const int FirstRepeatDelay = 400;
    private const int RepeatInterval = 70;

    private readonly System.Windows.Forms.Timer _repeat = new();
    private int _hover;    // +1 위, -1 아래, 0 없음
    private int _pressed;

    /// <summary>+ 를 눌렀을 때.</summary>
    public event EventHandler? Increase;

    /// <summary>− 를 눌렀을 때.</summary>
    public event EventHandler? Decrease;

    public BrushSizeStepper()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.Selectable, false);
        BackColor = Color.Transparent;
        Size = new Size(20, 30);
        Cursor = Cursors.Hand;
        _repeat.Tick += (_, _) =>
        {
            _repeat.Interval = RepeatInterval;
            Fire();
        };
    }

    private int PartAt(Point p) => p.Y < Height / 2 ? 1 : -1;

    private void Fire()
    {
        if (_pressed > 0) Increase?.Invoke(this, EventArgs.Empty);
        else if (_pressed < 0) Decrease?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        float half = Height / 2f;
        DrawPart(g, new RectangleF(1, 1, Width - 2, half - 1.5f), +1);
        DrawPart(g, new RectangleF(1, half + 0.5f, Width - 2, half - 1.5f), -1);
    }

    private void DrawPart(Graphics g, RectangleF r, int part)
    {
        var fill = _pressed == part ? Theme.AccentPressed : _hover == part ? Theme.RaisedHover : Theme.Raised;
        using (var path = Theme.RoundRect(r, 3))
        using (var brush = new SolidBrush(Enabled ? fill : Theme.Card))
            g.FillPath(brush, path);

        using var pen = new Pen(Enabled ? Theme.Text : Theme.TextFaint, 1.5f);
        float cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2, s = Math.Min(3.5f, r.Height / 3);
        g.DrawLine(pen, cx - s, cy, cx + s, cy);
        if (part > 0) g.DrawLine(pen, cx, cy - s, cx, cy + s);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int part = PartAt(e.Location);
        if (part != _hover) { _hover = part; Invalidate(); }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hover = 0;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left || !Enabled) return;
        _pressed = PartAt(e.Location);
        Invalidate();
        Fire();
        _repeat.Interval = FirstRepeatDelay;
        _repeat.Start();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _repeat.Stop();
        _pressed = 0;
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _repeat.Dispose();
        base.Dispose(disposing);
    }
}

/// <summary>도구 모음(ToolStrip)에 넣는 브러시 크기 + / − 버튼 (디자이너에서 배치 가능).</summary>
[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
public sealed class ToolStripBrushSize : ToolStripControlHost
{
    public ToolStripBrushSize()
        : base(new BrushSizeStepper())
    {
        AutoSize = false;
        Size = new Size(20, 30);
    }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BrushSizeStepper Stepper => (BrushSizeStepper)Control;
}
