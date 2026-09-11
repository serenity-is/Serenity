using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Serenity.Web.EsBuild;

public class EsBuildDownloaderTests
{
    private class FakePlatformInfo(string platform = "win32", string architecture = "x64") : IEsBuildPlatformInfo
    {
        public string Platform => platform;
        public string Architecture => architecture;
    }

    private class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responder(request));
        }
    }

    private static byte[] CreateGzippedTar(string executableName, string content)
    {
        using var tar = TarFileReaderTests.CreateTar((executableName, content));
        using var compressed = new System.IO.MemoryStream();
        using (var gz = new GZipStream(compressed, CompressionMode.Compress, leaveOpen: true))
            tar.CopyTo(gz);
        return compressed.ToArray();
    }

    private static HttpClient CreateHttpClient(byte[] gzippedTar, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(status)
        {
            Content = new ByteArrayContent(gzippedTar)
        });
        return new HttpClient(handler);
    }

    [Fact]
    public async Task GetLatestVersionAsync_Returns_Known_Version()
    {
        var downloader = new EsBuildDownloader(new FakePlatformInfo(), CreateHttpClient([]));

        Assert.Equal("0.28.2", await downloader.GetLatestVersionAsync("win32-x64"));
        Assert.Equal("0.28.2", downloader.GetLatestVersionSync("win32-x64"));
    }

    [Fact]
    public async Task DownloadAsync_Extracts_Executable()
    {
        var targetDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "esbuildtest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var gzippedTar = CreateGzippedTar("package/esbuild.exe", "BINARY");
            var downloader = new EsBuildDownloader(new FakePlatformInfo(), CreateHttpClient(gzippedTar));

            var path = await downloader.DownloadAsync(targetDirectory, "1.0.0");

            Assert.True(System.IO.File.Exists(path));
            Assert.Equal("BINARY", System.IO.File.ReadAllText(path));
            Assert.EndsWith("esbuild.exe", path);
        }
        finally
        {
            if (System.IO.Directory.Exists(targetDirectory))
                System.IO.Directory.Delete(targetDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task DownloadAsync_Returns_Existing_File()
    {
        var targetDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "esbuildtest-" + Guid.NewGuid().ToString("N"));
        try
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(targetDirectory, "1.0.0"));
            var existing = System.IO.Path.Combine(targetDirectory, "1.0.0", "esbuild.exe");
            System.IO.File.WriteAllText(existing, "EXISTING");
            var downloader = new EsBuildDownloader(new FakePlatformInfo(), CreateHttpClient([]));

            var path = await downloader.DownloadAsync(targetDirectory, "1.0.0");

            Assert.Equal(existing, path);
        }
        finally
        {
            if (System.IO.Directory.Exists(targetDirectory))
                System.IO.Directory.Delete(targetDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task DownloadAsync_Throws_When_No_Executable_In_Package()
    {
        var targetDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "esbuildtest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var gzippedTar = CreateGzippedTar("package/other.txt", "NOPE");
            var downloader = new EsBuildDownloader(new FakePlatformInfo(), CreateHttpClient(gzippedTar));

            await Assert.ThrowsAsync<Exception>(() => downloader.DownloadAsync(targetDirectory, "1.0.0"));
        }
        finally
        {
            if (System.IO.Directory.Exists(targetDirectory))
                System.IO.Directory.Delete(targetDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task DownloadAsync_Throws_On_Http_Error()
    {
        var downloader = new EsBuildDownloader(new FakePlatformInfo(),
            CreateHttpClient([], HttpStatusCode.InternalServerError));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            downloader.DownloadAsync(System.IO.Path.GetTempPath(), "1.0.0"));
    }

    [Fact]
    public void Download_Sync_Extracts_Executable()
    {
        var targetDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "esbuildtest-" + Guid.NewGuid().ToString("N"));
        try
        {
            var gzippedTar = CreateGzippedTar("package/esbuild.exe", "SYNC");
            var downloader = new EsBuildDownloader(new FakePlatformInfo(), CreateHttpClient(gzippedTar));

            var path = downloader.Download(targetDirectory, "1.0.0");

            Assert.True(System.IO.File.Exists(path));
        }
        finally
        {
            if (System.IO.Directory.Exists(targetDirectory))
                System.IO.Directory.Delete(targetDirectory, recursive: true);
        }
    }
}
