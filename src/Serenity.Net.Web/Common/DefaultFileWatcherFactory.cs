using System.Collections.Generic;

namespace Serenity.Web
{
    public class DefaultFileWatcherFactory : IFileWatcherFactory
    {
        private List<IFileWatcher> watchers;

        public DefaultFileWatcherFactory()
        {
            watchers = new List<IFileWatcher>();
        }

        public void KeepAlive(IFileWatcher fileWatcher)
        {
            if (!watchers.Contains(fileWatcher))
                watchers.Add(fileWatcher);
        }

        public IFileWatcher Create(string path, string filter)
        {
            return new FileWatcher(path, filter);
        }

        public IEnumerable<IFileWatcher> Watchers => watchers;
    }
}