using System.Collections;

namespace Serenity.Tests;

public class MockDirectoryContents(System.IO.Abstractions.IFileSystem fileSystem, string path) : IDirectoryContents
{
    public bool Exists => FileSystem.Directory.Exists(Path);

    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem;
    public string Path { get; } = path;

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