namespace NukkiStudio.App.Models;

/// <summary>
/// 모델 폴더 위치: 항상 exe와 같은 폴더의 models (휴대용 — 폴더째 옮겨도 그대로 동작).
/// 상대 경로("./models")는 실행 방법(바로가기 / 끌어다 놓기 / 연결 프로그램)에 따라 현재 작업 폴더가 달라지므로 쓰지 않고,
/// exe의 실제 위치(AppContext.BaseDirectory)를 기준으로 한다.
/// </summary>
public static class ModelLocation
{
    public static string Resolve() => Path.Combine(AppContext.BaseDirectory, "models");
}
