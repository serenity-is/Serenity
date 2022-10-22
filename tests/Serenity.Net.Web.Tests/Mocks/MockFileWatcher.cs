namespace Serenity.Tests;

public class MockFileWatcher : IFileWatcher
{
    private Action<string> changed;

    public IFileSystem FileSystem { get; }

    public MockFileWatcher(IFileSystem fileSystem, string path, string filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Filter = filter ?? throw new ArgumentNullException(nameof(filter));
    }

    public event Action<string> Changed
    {
        add { changed += value; }
        remove { changed -= value; }
    }

    public void RaiseChanged(string name)
    {
        changed?.Invoke(name);
    }

    public string Path { get; }
    public string Filter { get; }
}