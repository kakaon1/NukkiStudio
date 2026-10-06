using System.Drawing;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Editing;

/// <summary>확정된 객체 하나. Mask는 보정(확장/축소, 페더)까지 적용된 최종 마스크.</summary>
public sealed class SegmentedObject
{
    public string Name { get; set; }
    public Mask Mask { get; }
    public Color Color { get; }
    public bool Visible { get; set; } = true;

    public SegmentedObject(string name, Mask mask, Color color)
    {
        Name = name;
        Mask = mask;
        Color = color;
    }

    public override string ToString() => Name;
}
