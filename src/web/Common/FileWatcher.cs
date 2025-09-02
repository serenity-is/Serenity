using System.IO;

namespace Serenity.Web;

/// <summary>
/// Default file watcher implementation for physical file system
/// </summary>
public class FileWatcher : IFileWatcher, IDisposable
{
    private Action<string> changed;
    private bool disposed;
    private readonly FileSystemWatcher watcher;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="path">Watch path</param>
    /// <param name="filter">Watch filter</param>
    /// <exception cref="ArgumentNullException">One of the arguments is null</exception>
    public FileWatcher(string path, string filter)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Filter = filter ?? throw new ArgumentNullException(nameof(filter));

        if (!Directory.Exists(Path))
            return;

        watcher = new FileSystemWatcher(Path, Filter)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
        watcher.Changed += (s, e) => FileChanged(e.Name);
        watcher.Created += (s, e) => FileChanged(e.Name);
        watcher.Deleted += (s, e) => FileChanged(e.Name);
        watcher.Renamed += (s, e) => FileChanged(e.OldName);
        watcher.EnableRaisingEvents = true;
    }

    private void FileChanged(string name)
    {
        changed?.Invoke(name);
    }

    /// <inheritdoc/>
    public event Action<string> Changed
    {
        add { changed += value; }
        remove { changed -= value; }
    }

    /// <inheritdoc/>
    protected void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
                watcher?.Dispose();

            disposed = true;
        }
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void RaiseChanged(string name)
    {
        changed?.Invoke(name);
    }

    /// <inheritdoc/>
    public string Path { get; }
    
    /// <inheritdoc/>
    public string Filter { get; }
}