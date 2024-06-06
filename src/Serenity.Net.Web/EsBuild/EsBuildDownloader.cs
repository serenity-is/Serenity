using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Web.EsBuild;

internal class EsBuildDownloader(IEsBuildPlatformInfo platformInfo = null, HttpClient httpClient = null) : IEsBuildDownloader
{
    private readonly IEsBuildPlatformInfo platformInfo = platformInfo ?? new EsBuildPlatformInfo();
    private readonly HttpClient httpClient = httpClient ?? new HttpClient();
    private static readonly SemaphoreSlim sync = new(1);

    public async Task<string> DownloadAsync(string targetDirectory, string version)
    {
        var executable = platformInfo.Platform switch
        {
            "win32" => "esbuild.exe",
            _ => "esbuild"
        };

        await sync.WaitAsync();
        try
        {
            var binString = $"{platformInfo.Platform}-{platformInfo.Architecture}";
            version ??= await GetLatestVersionAsync(binString);

            var targetDir = Path.Combine(targetDirectory ??
                Path.Combine(Directory.GetCurrentDirectory(), "esbuild"), version!);
            var targetPath = Path.Combine(targetDir, executable);

            if (File.Exists(targetPath))
                return targetPath;

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var url = $"https://registry.npmjs.org/@esbuild/{binString}/-/{binString}-{version}.tgz";

            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var gzip = new GZipStream(stream, CompressionMode.Decompress);
            using var ms = new MemoryStream();
            gzip.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            foreach (var entry in TarFileReader.EnumerateEntries(ms))
            {
                if (!entry.Name.EndsWith(executable))
                    continue;

                using (var fs = File.Create(targetPath))
                    TarFileReader.CopyEntryTo(ms, entry, fs);

                if (!OperatingSystem.IsWindows())
                {
                    try
                    {
                        File.SetUnixFileMode(targetPath,
                            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                            UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
                            UnixFileMode.OtherRead);
                    }
                    catch
                    {
                    }
                }

                return targetPath;
            }
        }
        finally
        {
            sync.Release();
        }

        throw new Exception("Failed to find esbuild executable in the downloaded package.");
    }

    public string Download(string targetDirectory = null, string version = null) => DownloadAsync(targetDirectory, version).Result;

    public async Task<string> GetLatestVersionAsync(string binString)
    {
        return await Task.FromResult("0.21.4");
        // var response = await _httpClient.GetAsync($"https://registry.npmjs.org/{binString}/latest");
        // response.EnsureSuccessStatusCode();
        //
        // var content = await response.Content.ReadAsStringAsync();
        // using var jsonDocument = JsonDocument.Parse(content);
        // var version = jsonDocument.RootElement.GetProperty("version").GetString();
        // return version;
    }

    public string GetLatestVersionSync(string binString) => GetLatestVersionAsync(binString).Result;
}