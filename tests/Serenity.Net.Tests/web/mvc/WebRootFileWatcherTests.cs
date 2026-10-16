using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Serenity.Web;

public class WebRootFileWatcherTests : IDisposable
{
    private readonly string tempDir;

    public WebRootFileWatcherTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "wrft-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "a.css"), "a{}");
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch
        {
        }
    }

    private MockHostEnvironment CreateEnv(string environmentName)
    {
        return new MockHostEnvironment(tempDir)
        {
            EnvironmentName = environmentName,
            WebRootPath = tempDir,
            WebRootFileProvider = new PhysicalFileProvider(tempDir)
        };
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => new WebRootFileWatcher(null!));
    }

    [Fact]
    public void GetChangeToken_Returns_Token_In_Development_For_Existing_File()
    {
        using var watcher = new WebRootFileWatcher(CreateEnv("Development"));

        Assert.NotNull(watcher.GetChangeToken("a.css"));
    }

    [Fact]
    public void GetChangeToken_Returns_Null_In_Production()
    {
        using var watcher = new WebRootFileWatcher(CreateEnv("Production"));

        Assert.Null(watcher.GetChangeToken("a.css"));
    }

    [Fact]
    public void GetChangeToken_Returns_Null_For_Missing_File()
    {
        using var watcher = new WebRootFileWatcher(CreateEnv("Development"));

        Assert.Null(watcher.GetChangeToken("missing.css"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetChangeToken_Returns_Null_For_Empty_Path(string? path)
    {
        using var watcher = new WebRootFileWatcher(CreateEnv("Development"));

        Assert.Null(watcher.GetChangeToken(path!));
    }
}
