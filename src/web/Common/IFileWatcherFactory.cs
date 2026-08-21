namespace Serenity.Web;

/// <summary>
/// An abstract factory for creating file system watchers.
/// </summary>
public interface IFileWatcherFactory
{
    /// <summary>
    /// Creates a new file system watcher for the given path and filter.
    /// </summary>
    /// <param name="path">The directory path to watch.</param>
    /// <param name="filter">The file filter to watch for.</param>
    /// <returns>A new <see cref="IFileWatcher"/> instance.</returns>
    IFileWatcher Create(string path, string filter);

    /// <summary>
    /// Keeps the file watcher alive, usually by retaining its instance
    /// reference in the watcher factory.
    /// </summary>
    /// <param name="fileWatcher">The file watcher to keep alive.</param>
    void KeepAlive(IFileWatcher fileWatcher);

    /// <summary>
    /// Gets the list of stored file watchers.
    /// </summary>
    IEnumerable<IFileWatcher> Watchers { get; }
}