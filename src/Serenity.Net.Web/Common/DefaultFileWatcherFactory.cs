namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IFileWatcherFactory"/>
/// </summary>
public class DefaultFileWatcherFactory : IFileWatcherFactory
{
    private readonly List<IFileWatcher> watchers;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public DefaultFileWatcherFactory()
    {
        watchers = new List<IFileWatcher>();
    }

    /// <inheritdoc/>
    public void KeepAlive(IFileWatcher fileWatcher)
    {
        if (!watchers.Contains(fileWatcher))
            watchers.Add(fileWatcher);
    }

    /// <inheritdoc/>
    public IFileWatcher Create(string path, string filter)
    {
        return new FileWatcher(path, filter);
    }

    /// <inheritdoc/>
    public IEnumerable<IFileWatcher> Watchers => watchers;
}