using System.Collections;

namespace Serenity.Tests;

public class MockDirectoryContents : IDirectoryContents
{
    public MockDirectoryContents(System.IO.Abstractions.IFileSystem fileSystem, string path)
    {
        FileSystem = fileSystem;
        Path = path;
    }

    public bool Exists => FileSystem.Directory.Exists(Path);

    public System.IO.Abstractions.IFileSystem FileSystem { get; }
    public string Path { get; }

    public IEnumerator<Microsoft.Extensions.FileProviders.IFileInfo> GetEnumerator()
    {
        return FileSystem.Directory.GetDirectories(Path)
            .Select(x => new MockFileInfo(FileSystem, x, true))
            .Concat(FileSystem.Directory.GetFiles(Path)
                .Select(x => new MockFileInfo(FileSystem, x, false))).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}