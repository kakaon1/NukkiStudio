using System.Drawing.Drawing2D;
using System.Reflection;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>
/// 도움말 창. exe에 포함된 docs/사용설명서.md를 읽어 "## 제목" 단위로 나누고,
/// 간단한 마크다운(제목 / 목록 / 표 / 굵게)을 서식 있는 텍스트로 보여 준다.
/// </summary>
public partial class HelpForm : Form
{
    private sealed record Section(string Title, List<string> Lines)
    {
        public override string ToString() => Title;
    }

    public HelpForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        foreach (var section in LoadSections()) lstSections.Items.Add(section);
        if (lstSections.Items.Count > 0) lstSections.SelectedIndex = 0;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    /// <summary>특정 제목의 섹션을 선택해서 연다 (예: "단축키 전체").</summary>
    public void SelectSection(string title)
    {
        for (int i = 0; i < lstSections.Items.Count; i++)
        {
            if (lstSections.Items[i] is Section s && s.Title.Contains(title, StringComparison.Ordinal))
            {
                lstSections.SelectedIndex = i;
                return;
            }
        }
    }

    private static List<Section> LoadSections()
    {
        var sections = new List<Section>();
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Help.manual.md");
        if (stream is null)
        {
            sections.Add(new Section("도움말", new List<string> { "도움말 문서를 찾을 수 없습니다." }));
            return sections;
        }

        using var reader = new StreamReader(stream);
        Section? current = null;
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                current = new Section(line[3..].Trim(), new List<string>());
                sections.Add(current);
            }
            else if (current is not null)
            {
                current.Lines.Add(line);
            }
        }
        return sections;
    }

    private void lstSections_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstSections.SelectedItem is Section section) Render(section);
    }

    // ---------------- 간단한 마크다운 렌더링 ----------------

    private void Render(Section section)
    {
        var rtb = txtContent;
        rtb.SuspendLayout();
        rtb.Clear();
        var family = Font.FontFamily;
        using var titleFont = new Font(family, 15f, FontStyle.Bold);
        using var headFont = new Font(family, 11f, FontStyle.Bold);
        using var bodyFont = new Font(family, 10f);
        using var boldFont = new Font(family, 10f, FontStyle.Bold);

        rtb.SelectionIndent = 18;
        rtb.SelectionRightIndent = 18;
        Append("\n", bodyFont, Theme.Text);
        Append(section.Title + "\n\n", titleFont, Theme.Text);

        var table = new List<string[]>();
        foreach (var raw in section.Lines.Append(""))
        {
            var line = raw.TrimEnd();

            // 표: 줄을 모았다가 표가 끝나면 한 번에 출력
            if (line.StartsWith('|'))
            {
                var cells = line.Trim('|').Split('|').Select(c => c.Trim()).ToArray();
                if (!cells.All(c => c.Length > 0 && c.All(ch => ch is '-' or ':'))) table.Add(cells);
                continue;
            }
            if (table.Count > 0)
            {
                RenderTable(table, bodyFont, boldFont);
                table.Clear();
            }

            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                Append("\n" + line[4..] + "\n", headFont, Theme.AccentHover);
            }
            else if (line.StartsWith("- ", StringComparison.Ordinal))
            {
                Append("   •  ", bodyFont, Theme.Accent);
                AppendInline(line[2..] + "\n", bodyFont, boldFont);
            }
            else if (line.Length > 2 && char.IsDigit(line[0]) && line[1] == '.')
            {
                Append("   " + line[..2] + " ", boldFont, Theme.Accent);
                AppendInline(line[2..].TrimStart() + "\n", bodyFont, boldFont);
            }
            else if (line.Length == 0)
            {
                Append("\n", bodyFont, Theme.Text);
            }
            else
            {
                AppendInline(line + "\n", bodyFont, boldFont);
            }
        }

        rtb.SelectionStart = 0;
        rtb.ScrollToCaret();
        rtb.ResumeLayout();
    }

    private void RenderTable(List<string[]> rows, Font body, Font bold)
    {
        // 첫 줄은 머리글. 열 너비를 맞추기 위해 탭 정지 위치 지정
        int columns = rows.Max(r => r.Length);
        var tabs = new int[Math.Max(0, columns - 1)];
        for (int i = 0; i < tabs.Length; i++) tabs[i] = 190 * (i + 1);

        for (int r = 0; r < rows.Count; r++)
        {
            txtContent.SelectionTabs = tabs;
            var row = rows[r];
            for (int c = 0; c < row.Length; c++)
            {
                string text = Strip(row[c]) + (c < row.Length - 1 ? "\t" : "\n");
                if (r == 0) Append(text, bold, Theme.TextDim);
                else Append(text, c == 0 ? bold : body, c == 0 ? Theme.Text : Theme.TextDim);
            }
        }
        Append("\n", body, Theme.Text);
    }

    /// <summary>**굵게** 와 `코드` 표시를 처리해서 추가.</summary>
    private void AppendInline(string text, Font body, Font bold)
    {
        var parts = text.Split("**");
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Replace("`", "");
            if (part.Length == 0) continue;
            bool isBold = i % 2 == 1;
            Append(part, isBold ? bold : body, isBold ? Theme.Text : Color.FromArgb(205, 205, 212));
        }
    }

    private static string Strip(string s) => s.Replace("**", "").Replace("`", "");

    private void Append(string text, Font font, Color color)
    {
        txtContent.SelectionStart = txtContent.TextLength;
        txtContent.SelectionLength = 0;
        txtContent.SelectionFont = font;
        txtContent.SelectionColor = color;
        txtContent.AppendText(text);
    }

    private void lstSections_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var g = e.Graphics;
        using (var back = new SolidBrush(lstSections.BackColor)) g.FillRectangle(back, e.Bounds);
        if (e.Index < 0 || lstSections.Items[e.Index] is not Section section) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        bool selected = (e.State & DrawItemState.Selected) != 0;
        if (selected)
        {
            var row = new RectangleF(e.Bounds.X + 4, e.Bounds.Y + 3, e.Bounds.Width - 8, e.Bounds.Height - 6);
            using var path = Theme.RoundRect(row, 6);
            using var fill = new SolidBrush(Theme.Raised);
            g.FillPath(fill, path);
            using var bar = new SolidBrush(Theme.Accent);
            g.FillRectangle(bar, row.X, row.Y + 8, 3, row.Height - 16);
        }

        var textRect = new Rectangle(e.Bounds.X + 16, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
        TextRenderer.DrawText(g, section.Title, lstSections.Font, textRect, selected ? Theme.Text : Theme.TextDim,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    private void btnAbout_Click(object? sender, EventArgs e)
    {
        using var about = new AboutForm();
        about.ShowDialog(this);
    }
}
