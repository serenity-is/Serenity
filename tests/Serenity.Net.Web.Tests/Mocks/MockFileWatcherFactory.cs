namespace Serenity.Tests;

public class MockFileWatcherFactory : IFileWatcherFactory
{
    private readonly List<IFileWatcher> watchers;

    public System.IO.Abstractions.IFileSystem FileSystem { get; }

    public MockFileWatcherFactory(System.IO.Abstractions.IFileSystem fileSystem)
    {
        FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        watchers = new List<IFileWatcher>();
    }

    public void KeepAlive(IFileWatcher fileWatcher)
    {
        if (!watchers.Contains(fileWatcher))
            watchers.Add(fileWatcher);
    }

    public IFileWatcher Create(string path, string filter)
    {
        return new MockFileWatcher(FileSystem, path, filter);
    }

    public IEnumerable<IFileWatcher> Watchers => watchers;
}