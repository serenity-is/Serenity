namespace Serenity.Tests
{
    public class MockFileProvider : IFileProvider
    {
        public IFileSystem FileSystem { get; }
        public string Root { get; }

        public MockFileProvider(string root = @"C:\Testing\Test\", IFileSystem fileSystem = null)
        {
            FileSystem = fileSystem ?? new MockFileSystem();
            Root = root;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new MockDirectoryContents(FileSystem, FileSystem.Path.Combine(Root, subpath));
        }

        public Microsoft.Extensions.FileProviders.IFileInfo GetFileInfo(string subpath)
        {
            return new MockFileInfo(FileSystem, FileSystem.Path.Combine(Root, subpath), false);
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}