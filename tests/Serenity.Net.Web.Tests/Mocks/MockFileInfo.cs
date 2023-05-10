namespace Serenity.Tests;

public class MockFileInfo : Microsoft.Extensions.FileProviders.IFileInfo
{
    public MockFileInfo(System.IO.Abstractions.IFileSystem fileSystem, string path, bool isDirectory)
    {
        FileSystem = fileSystem;
        Path = path;
        IsDirectory = isDirectory;
    }

    public System.IO.Abstractions.IFileSystem FileSystem { get; }
    public string Path { get; }

    public bool Exists => IsDirectory ? FileSystem.Directory.Exists(Path) : FileSystem.File.Exists(Path);

    public bool IsDirectory { get; set; }

    public DateTimeOffset LastModified => IsDirectory ?
        FileSystem.Directory.GetLastWriteTime(Path) :
        FileSystem.File.GetLastWriteTime(Path);

    public long Length => IsDirectory ? 0 :
        FileSystem.FileInfo.New(Path).Length;

    public string Name => FileSystem.Path.GetFileName(Path);

    public string PhysicalPath => null;

    public System.IO.Stream CreateReadStream()
    {
        return FileSystem.File.OpenRead(Path);
    }
}