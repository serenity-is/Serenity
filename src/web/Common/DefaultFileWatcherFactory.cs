namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IFileWatcherFactory"/> that keeps
/// created watchers alive for the lifetime of the factory.
/// </summary>
public class DefaultFileWatcherFactory : IFileWatcherFactory
{
    private readonly List<IFileWatcher> watchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultFileWatcherFactory"/> class.
    /// </summary>
    public DefaultFileWatcherFactory()
    {
        watchers = [];
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