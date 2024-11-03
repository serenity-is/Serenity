namespace Serenity.Tests;

public class MockFileInfo(System.IO.Abstractions.IFileSystem fileSystem, string path, bool isDirectory) : Microsoft.Extensions.FileProviders.IFileInfo
{
    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem;
    public string Path { get; } = path;

    public bool Exists => IsDirectory ? FileSystem.Directory.Exists(Path) : FileSystem.File.Exists(Path);

    public bool IsDirectory { get; set; } = isDirectory;

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