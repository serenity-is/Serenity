namespace Serenity.Tests;

public class MockFileWatcherFactory(System.IO.Abstractions.IFileSystem fileSystem) : IFileWatcherFactory
{
    private readonly List<IFileWatcher> watchers = [];

    public System.IO.Abstractions.IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

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