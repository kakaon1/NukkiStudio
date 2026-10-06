using System.ComponentModel;
using System.Drawing.Drawing2D;
using NukkiStudio.App.UI;

namespace NukkiStudio.App.Controls;

/// <summary>
/// 플랫 디자인 슬라이더 (TrackBar 대체). Minimum / Maximum / Value / ValueChanged 이름은 TrackBar와 같다.
/// 드래그, 클릭, 마우스 휠, 방향키로 조절한다.
/// </summary>
[DefaultEvent(nameof(ValueChanged))]
public sealed class FlatSlider : Control
{
    private int _minimum;
    private int _maximum = 100;
    private int _value;
    private bool _dragging;
    private bool _hover;

    public FlatSlider()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
        TabStop = true;
        Cursor = Cursors.Hand;
        Size = new Size(200, 24);
    }

    public event EventHandler? ValueChanged;

    [Category("Behavior"), DefaultValue(0)]
    public int Minimum
    {
        get => _minimum;
        set { _minimum = value; if (_maximum < value) _maximum = value; Value = _value; Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(100)]
    public int Maximum
    {
        get => _maximum;
        set { _maximum = value; if (_minimum > value) _minimum = value; Value = _value; Invalidate(); }
    }

    [Category("Behavior"), DefaultValue(0)]
    public int Value
    {
        get => _value;
        set
        {
            int v = Math.Clamp(value, _minimum, _maximum);
            if (v == _value) return;
            _value = v;
            Invalidate();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Category("Behavior"), DefaultValue(1)]
    public int SmallChange { get; set; } = 1;

    [Category("Behavior"), DefaultValue(5)]
    public int LargeChange { get; set; } = 5;

    /// <summary>true면 0 위치에서 양쪽으로 채운다 (확장 / 축소처럼 음수 범위가 있는 값).</summary>
    [Category("Appearance"), DefaultValue(false)]
    public bool FillFromZero { get; set; }

    private const float ThumbRadius = 7f;

    private float TrackLeft => ThumbRadius + 1;
    private float TrackRight => Width - ThumbRadius - 1;

    private float ValueToX(int v)
    {
        if (_maximum == _minimum) return TrackLeft;
        return TrackLeft + (TrackRight - TrackLeft) * (v - _minimum) / (_maximum - _minimum);
    }

    private int XToValue(float x)
    {
        float t = Math.Clamp((x - TrackLeft) / Math.Max(1f, TrackRight - TrackLeft), 0f, 1f);
        return (int)MathF.Round(_minimum + t * (_maximum - _minimum));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        float cy = Height / 2f;
        bool enabled = Enabled;

        using (var track = new SolidBrush(Theme.Raised))
        using (var path = Theme.RoundRect(new RectangleF(TrackLeft, cy - 2, TrackRight - TrackLeft, 4), 2))
            g.FillPath(track, path);

        float x = ValueToX(_value);
        float from = FillFromZero ? ValueToX(Math.Clamp(0, _minimum, _maximum)) : TrackLeft;
        float a = Math.Min(from, x), b = Math.Max(from, x);
        if (b - a > 0.5f)
        {
            using var fill = new SolidBrush(enabled ? Theme.Accent : Theme.TextFaint);
            using var path = Theme.RoundRect(new RectangleF(a, cy - 2, b - a, 4), 2);
            g.FillPath(fill, path);
        }

        float r = (_dragging || _hover || Focused) ? ThumbRadius : ThumbRadius - 1;
        using (var ring = new SolidBrush(enabled ? Theme.Accent : Theme.TextFaint))
            g.FillEllipse(ring, x - r, cy - r, r * 2, r * 2);
        using (var knob = new SolidBrush(Color.White))
            g.FillEllipse(knob, x - r + 2.5f, cy - r + 2.5f, r * 2 - 5, r * 2 - 5);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left) return;
        Focus();
        _dragging = true;
        Value = XToValue(e.X);
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragging) Value = XToValue(e.X);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _hover = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hover = false;
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        Value += e.Delta > 0 ? SmallChange : -SmallChange;
        if (e is HandledMouseEventArgs h) h.Handled = true;
    }

    protected override bool IsInputKey(Keys keyData) =>
        keyData is Keys.Left or Keys.Right or Keys.Up or Keys.Down || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.KeyCode)
        {
            case Keys.Left or Keys.Down: Value -= SmallChange; break;
            case Keys.Right or Keys.Up: Value += SmallChange; break;
            case Keys.PageDown: Value -= LargeChange; break;
            case Keys.PageUp: Value += LargeChange; break;
            case Keys.Home: Value = _minimum; break;
            case Keys.End: Value = _maximum; break;
            default: return;
        }
        e.Handled = true;
    }

    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }

    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

    protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }
}
