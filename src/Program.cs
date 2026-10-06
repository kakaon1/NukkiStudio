namespace NukkiStudio.App;

internal static class Program
{
    /// <summary>처리되지 않은 예외 기록 파일.</summary>
    public static string ErrorLogPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NukkiStudio", "error.log");

    [STAThread]
    static void Main(string[] args)
    {
        Application.SetDefaultFont(new System.Drawing.Font("Malgun Gothic", 9F));
        ApplicationConfiguration.Initialize();

        // 예상하지 못한 오류가 나도 프로그램이 닫히지 않게 하고, 원인은 파일에 남긴다
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ReportError(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => { if (e.ExceptionObject is Exception ex) LogError(ex); };

        // exe에 이미지 파일 / 폴더를 끌어다 놓거나 "연결 프로그램"으로 열면 인수가 경로 목록
        Application.Run(new MainForm { InitialPaths = args.Where(a => File.Exists(a) || Directory.Exists(a)).ToArray() });
    }

    private static void ReportError(Exception ex)
    {
        LogError(ex);
        MessageBox.Show($"작업 중 오류가 발생했습니다. 작업은 계속할 수 있습니다.\n\n{ex.Message}\n\n기록: {ErrorLogPath}",
            "Nukki Studio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void LogError(Exception ex)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ErrorLogPath)!);
            File.AppendAllText(ErrorLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}\r\n\r\n");
        }
        catch (Exception)
        {
            // 기록 실패는 무시
        }
    }
}
