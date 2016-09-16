using System;
using System.IO;
using System.Web.Hosting;

namespace Serenity.Web
{
    public static class CssFileWatcher
    {
        public static void WatchForChanges(string path = "~/Content")
        {
            if (path.StartsWith("~/"))
                path = HostingEnvironment.MapPath(path);

            var sw = new FileSystemWatcher(path);
            sw.IncludeSubdirectories = true;
            sw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            sw.Changed += (s, e) => Changed(e.Name);
            sw.Created += (s, e) => Changed(e.Name);
            sw.Deleted += (s, e) => Changed(e.Name);
            sw.Renamed += (s, e) => Changed(e.OldName);
            sw.EnableRaisingEvents = true;
        }

        private static void Changed(string name)
        {
            var extension = Path.GetExtension(name);
            if (extension == null || string.Compare(extension, ".css", StringComparison.OrdinalIgnoreCase) != 0)
                return;

            ContentHashCache.ScriptsChanged();
            ScriptBundleManager.ScriptsChanged();
        }
    }
}