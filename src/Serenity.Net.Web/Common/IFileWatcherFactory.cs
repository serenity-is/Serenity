using System.Collections.Generic;

namespace Serenity.Web
{
    public interface IFileWatcherFactory
    {
        IFileWatcher Create(string path, string filter);
        void KeepAlive(IFileWatcher fileWatcher);
        IEnumerable<IFileWatcher> Watchers { get; }
    }
}