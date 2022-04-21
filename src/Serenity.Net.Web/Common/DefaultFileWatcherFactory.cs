namespace Serenity.Web
{
    public class DefaultFileWatcherFactory : IFileWatcherFactory
    {
        private readonly List<IFileWatcher> watchers;

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