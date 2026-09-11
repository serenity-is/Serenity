using Microsoft.Extensions.Options;

namespace Serenity.Reporting;

public class WKHtmlToPdfConverterTests
{
    private sealed class FakeServerFileSystem : Serenity.IFileSystem
    {
        public HashSet<string> Files { get; } = new(StringComparer.OrdinalIgnoreCase);

        public bool FileExists(string path) => Files.Contains(path);

        public System.IO.Stream CreateFile(string path, bool overwrite = true) => throw new NotImplementedException();
        public void CreateDirectory(string path) => throw new NotImplementedException();
        public void DeleteDirectory(string path, bool recursive = false) => throw new NotImplementedException();
        public void DeleteFile(string path) => throw new NotImplementedException();
        public bool DirectoryExists(string path) => false;
        public string[] GetDirectories(string path, string searchPattern = "*", bool recursive = false) => [];
        public string[] GetFiles(string path, string searchPattern = "*", bool recursive = false) => [];
        public long GetFileSize(string path) => throw new NotImplementedException();
        public string GetFullPath(string path) => throw new NotImplementedException();
        public DateTime GetLastWriteTimeUtc(string path) => throw new NotImplementedException();
        public string GetRelativePath(string relativeTo, string path) => throw new NotImplementedException();
        public System.IO.Stream OpenRead(string path) => throw new NotImplementedException();
        public byte[] ReadAllBytes(string path) => throw new NotImplementedException();
        public string ReadAllText(string path, Encoding? encoding = null) => throw new NotImplementedException();
        public void WriteAllBytes(string path, byte[] content) => throw new NotImplementedException();
        public void WriteAllText(string path, string content, Encoding? encoding = null) => throw new NotImplementedException();
    }

    [Fact]
    public void GetExecutablePath_Returns_Options_Path_When_Exists()
    {
        var fileSystem = new FakeServerFileSystem();
        var path = "/tools/wkhtmltopdf.exe";
        fileSystem.Files.Add(path);
        var options = Microsoft.Extensions.Options.Options.Create(new WKHtmlToPdfSettings
        {
            ExecutablePath = path
        });
        var converter = new WKHtmlToPdfConverter(options, null, fileSystem);

        Assert.Equal(path, converter.GetExecutablePath());
        Assert.Equal(path, converter.GetExecutablePath());
    }

    [Fact]
    public void GetExecutablePath_Returns_Null_When_Not_Found()
    {
        var fileSystem = new FakeServerFileSystem();
        var options = Microsoft.Extensions.Options.Options.Create(new WKHtmlToPdfSettings
        {
            ExecutablePath = "missing/wkhtmltopdf.exe"
        });
        var converter = new WKHtmlToPdfConverter(options, null, fileSystem);

        Assert.Null(converter.GetExecutablePath());
    }

    [Fact]
    public void GetExecutablePath_Finds_In_Content_Root()
    {
        var env = new MockHostEnvironment();
        var exeName = OperatingSystem.IsWindows() ? "wkhtmltopdf.exe" : "wkhtmltopdf";
        var path = env.Path.Combine(env.ContentRootPath, "App_Data", "Reporting", exeName);
        var fileSystem = new FakeServerFileSystem();
        fileSystem.Files.Add(path);
        var options = Microsoft.Extensions.Options.Options.Create(new WKHtmlToPdfSettings());
        var converter = new WKHtmlToPdfConverter(options, env, fileSystem);

        Assert.Equal(path, converter.GetExecutablePath());
    }

    [Fact]
    public void Convert_Throws_When_Executable_Missing()
    {
        var fileSystem = new FakeServerFileSystem();
        var options = Microsoft.Extensions.Options.Options.Create(new WKHtmlToPdfSettings
        {
            ExecutablePath = "missing/wkhtmltopdf.exe"
        });
        var converter = new WKHtmlToPdfConverter(options, null, fileSystem);

        var exception = Assert.Throws<ValidationError>(() => converter.Convert(new HtmlToPdfOptions()));
        Assert.Contains("Can't locate wkhtmltopdf executable", exception.Message);
    }
}
