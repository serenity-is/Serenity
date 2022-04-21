using System.IO;

namespace Serenity.Web
{
    public class FileWatcher : IFileWatcher, IDisposable
    {
        private Action<string> changed;
        private bool disposed;
        private readonly FileSystemWatcher watcher;

        public FileWatcher(string path, string filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            Path = path ?? throw new ArgumentNullException(nameof(path));
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));

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

        public event Action<string> Changed
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    watcher?.Dispose();

                disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void RaiseChanged(string name)
        {
            changed?.Invoke(name);
        }

        public string Path { get; }
        public string Filter { get; }
    }
}