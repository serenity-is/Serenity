namespace Serenity.Web;

/// <summary>
/// File system watcher abstraction
/// </summary>
public interface IFileWatcher
{
    /// <summary>
    /// Event that is raised when a file changed
    /// </summary>
    event Action<string> Changed;

    /// <summary>
    /// Raises the change even
    /// </summary>
    /// <param name="name">File name</param>
    void RaiseChanged(string name);

    /// <summary>
    /// Watch path
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Watch filter
    /// </summary>
    public string Filter { get; }
}