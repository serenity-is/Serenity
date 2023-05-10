namespace Serenity.Tests;

public class MockHostEnvironment : IWebHostEnvironment
{
    public System.IO.Abstractions.IFileSystem FileSystem { get; }
    public string WebRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
    public string ApplicationName { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
    public string ContentRootPath { get; set; }
    public string EnvironmentName { get; set; }
    public System.IO.Abstractions.IPath Path => FileSystem.Path;
    public System.IO.Abstractions.IFile File => FileSystem.File;
    public System.IO.Abstractions.IDirectory Directory => FileSystem.Directory;

    public void AddWebFile(string path, string text)
    {
        var file = Path.Combine(WebRootPath, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        File.WriteAllText(file, text);
    }

    public MockHostEnvironment(string root = @"/Testing/Test/", System.IO.Abstractions.IFileSystem fileSystem = null)
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