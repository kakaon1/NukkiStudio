using System.Diagnostics;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>
/// 오래 걸리는 작업(고품질 지우기 등) 동안 띄우는 "작업 중" 창. 작업이 끝나면 코드에서 닫는다.
/// 사용자가 닫을 수 없다 (닫기 버튼 없음, Alt+F4 무시).
/// </summary>
public partial class BusyForm : Form
{
    private readonly Stopwatch _elapsed = new();
    private bool _allowClose;

    public BusyForm()
    {
        InitializeComponent();
        if (DesignMode) return;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    /// <summary>
    /// 작업이 delayMs보다 오래 걸릴 때만 창을 띄우고(빠른 작업은 깜빡임 없음), 끝나면 닫는다.
    /// owner 가운데에 표시되며, 작업 중 예외는 그대로 호출한 쪽으로 전달된다.
    /// </summary>
    public static async Task<T> RunAsync<T>(Form owner, string message, string detail, Func<Task<T>> work, int delayMs = 300)
    {
        var task = work();
        if (await Task.WhenAny(task, Task.Delay(delayMs)) == task) return await task;

        using var form = new BusyForm();
        form.lblMessage.Text = message;
        form.lblDetail.Text = detail;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(
            owner.Left + (owner.Width - form.Width) / 2,
            owner.Top + (owner.Height - form.Height) / 2);
        form._elapsed.Start();
        form.tmrElapsed.Start();
        form.Show(owner);
        try
        {
            return await task;
        }
        finally
        {
            form.tmrElapsed.Stop();
            form._allowClose = true;
            form.Close();
            owner.Activate();
        }
    }

    private void tmrElapsed_Tick(object? sender, EventArgs e) =>
        lblElapsed.Text = $"{_elapsed.Elapsed.TotalSeconds:0.0}초";

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_allowClose && e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
        base.OnFormClosing(e);
    }
}
