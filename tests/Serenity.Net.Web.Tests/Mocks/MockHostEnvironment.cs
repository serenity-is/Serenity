namespace Serenity.Tests;

public class MockHostEnvironment : IWebHostEnvironment
{
    public IFileSystem FileSystem { get; }
    public string WebRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
    public string ApplicationName { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
    public string ContentRootPath { get; set; }
    public string EnvironmentName { get; set; }
    public IPath Path => FileSystem.Path;
    public IFile File => FileSystem.File;
    public IDirectory Directory => FileSystem.Directory;

    public void AddWebFile(string path, string text)
    {
        var file = Path.Combine(WebRootPath, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        File.WriteAllText(file, text);
    }

    public MockHostEnvironment(string root = @"C:\Testing\Test\", IFileSystem fileSystem = null)
    {
        EnvironmentName = "Development";
        ApplicationName = "Test";
        FileSystem = fileSystem ?? new MockFileSystem();
        ContentRootPath = root;
        FileSystem.Directory.CreateDirectory(ContentRootPath);
        ContentRootFileProvider = new MockFileProvider(ContentRootPath, FileSystem);
        WebRootPath = Path.Combine(ContentRootPath, "wwwroot");
        WebRootFileProvider = new MockFileProvider(WebRootPath, FileSystem);
        FileSystem.Directory.CreateDirectory(WebRootPath);
    }
}