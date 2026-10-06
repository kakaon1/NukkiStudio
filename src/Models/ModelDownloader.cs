using System.IO.Compression;
using System.Net.Http;

namespace NukkiStudio.App.Models;

/// <summary>다운로드 진행 상황 (현재 모델 / 전체).</summary>
public readonly record struct DownloadProgress(ModelPackage Package, int Index, int Count, long Received, long? Total, string Stage);

/// <summary>
/// 모델 파일을 내려받아 models 폴더에 설치한다.
/// 임시 파일(.part)에 받은 뒤 완료되면 옮기므로, 중간에 취소 / 끊김이 나도 반쯤 받은 모델이 남지 않는다.
/// </summary>
public static class ModelDownloader
{
    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("NukkiStudio/1.0");
        return client;
    }

    public static async Task InstallAsync(IReadOnlyList<ModelPackage> packages, string modelsDirectory, IProgress<DownloadProgress> progress, CancellationToken token)
    {
        for (int i = 0; i < packages.Count; i++)
        {
            var package = packages[i];
            var folder = Path.Combine(modelsDirectory, package.Folder);
            Directory.CreateDirectory(folder);
            var temp = Path.Combine(folder, $".{package.Id}.download.part");
            try
            {
                await DownloadFileAsync(package, i, packages.Count, temp, progress, token);
                progress.Report(new DownloadProgress(package, i, packages.Count, 1, 1, "설치 중"));
                if (package.IsZip) ExtractZip(temp, folder, package.Files);
                else File.Move(temp, Path.Combine(folder, package.Files[0]), overwrite: true);

                if (!package.IsInstalled(modelsDirectory))
                    throw new InvalidDataException($"{package.Title}: 받은 파일에 필요한 파일이 없습니다 ({string.Join(", ", package.Files)}).");
            }
            finally
            {
                TryDelete(temp);
            }
        }
    }

    private static async Task DownloadFileAsync(ModelPackage package, int index, int count, string path, IProgress<DownloadProgress> progress, CancellationToken token)
    {
        using var response = await Http.GetAsync(package.Url, HttpCompletionOption.ResponseHeadersRead, token);
        response.EnsureSuccessStatusCode();
        long? total = response.Content.Headers.ContentLength;

        await using var source = await response.Content.ReadAsStreamAsync(token);
        await using var target = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16, useAsync: true);
        var buffer = new byte[1 << 16];
        long received = 0;
        var lastReport = Environment.TickCount64;
        progress.Report(new DownloadProgress(package, index, count, 0, total, "다운로드 중"));
        int read;
        while ((read = await source.ReadAsync(buffer, token)) > 0)
        {
            await target.WriteAsync(buffer.AsMemory(0, read), token);
            received += read;
            // 화면 갱신은 0.1초에 한 번
            if (Environment.TickCount64 - lastReport >= 100)
            {
                lastReport = Environment.TickCount64;
                progress.Report(new DownloadProgress(package, index, count, received, total, "다운로드 중"));
            }
        }
        progress.Report(new DownloadProgress(package, index, count, received, total ?? received, "다운로드 중"));
    }

    /// <summary>압축 안에서 이름이 files에 있는 파일만 폴더에 바로 푼다 (하위 폴더 구조 무시).</summary>
    private static void ExtractZip(string zipPath, string folder, string[] files)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name)) continue; // 폴더 항목
            if (!files.Contains(entry.Name, StringComparer.OrdinalIgnoreCase)) continue;
            entry.ExtractToFile(Path.Combine(folder, entry.Name), overwrite: true);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception)
        {
            // 임시 파일 삭제 실패는 무시
        }
    }
}
