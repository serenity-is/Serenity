namespace Serenity.Web;

/// <summary>
/// Abstraction for a file system watcher.
/// </summary>
public interface IFileWatcher
{
    /// <summary>
    /// Raised when a watched file changes.
    /// </summary>
    event Action<string> Changed;

    /// <summary>
    /// Raises the <see cref="Changed"/> event for the specified file.
    /// </summary>
    /// <param name="name">The name of the changed file.</param>
    void RaiseChanged(string name);

    /// <summary>
    /// Gets the directory path being watched.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the file filter being watched.
    /// </summary>
    public string Filter { get; }
}