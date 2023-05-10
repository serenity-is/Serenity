namespace Serenity.Web;

/// <summary>
/// An abstract factory to create file system watchers
/// </summary>
public interface IFileWatcherFactory
{
    /// <summary>
    /// Creates a new file system watcher for path and filter
    /// </summary>
    /// <param name="path">Watch path</param>
    /// <param name="filter">Watch filter</param>
    IFileWatcher Create(string path, string filter);

    /// <summary>
    /// Keeps the file watcher alive, usually keeping
    /// its instance reference in the watcher factory
    /// </summary>
    /// <param name="fileWatcher">File watcher</param>
    void KeepAlive(IFileWatcher fileWatcher);

    /// <summary>
    /// Gets list of stored file watchers
    /// </summary>
    IEnumerable<IFileWatcher> Watchers { get; }
}