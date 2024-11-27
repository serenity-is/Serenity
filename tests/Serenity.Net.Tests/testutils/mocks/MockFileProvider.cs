namespace Serenity.Tests;

public class MockFileProvider(string root = @"/Testing/Test/", System.IO.Abstractions.IFileSystem fileSystem = null) : IFileProvider
{
    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem ?? new MockFileSystem();
    public string Root { get; } = root;

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return new MockDirectoryContents(FileSystem, FileSystem.Path.Combine(Root, subpath));
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        return new MockFileInfo(FileSystem, FileSystem.Path.Combine(Root, subpath), false);
    }

    public IChangeToken Watch(string filter)
    {
        throw new NotImplementedException();
    }
}