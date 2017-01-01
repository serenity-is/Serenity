using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace Serenity.Web
{
    public class TemplateScriptRegistrar
    {
        const string TemplateSuffix = ".Template.html";

        private ConcatenatedScript bundle;
        private Dictionary<string, TemplateScript> scriptByKey = new Dictionary<string, TemplateScript>(StringComparer.OrdinalIgnoreCase);

        private static string GetKey(string filename)
        {
            string key = Path.GetFileName(filename);

            if (!key.EndsWith(TemplateSuffix, StringComparison.OrdinalIgnoreCase))
                return null;

            return key.Substring(0, key.Length - TemplateSuffix.Length);
        }

        private void WatchForChanges(string path)
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

        private void Changed(string name)
        {
            string key = GetKey(name);
            if (key == null)
                return;

            TemplateScript ts;
            if (scriptByKey.TryGetValue(key, out ts))
                ts.Changed();

            if (bundle != null)
                bundle.Changed();
        }

        public void Initialize(string[] rootUrls, bool watchForChanges = true)
        {
            var bundleList = new List<Func<string>>();

            foreach (var rootUrl in rootUrls)
            {
                var path = rootUrl;
                if (path.StartsWith("~/"))
                    path = HostingEnvironment.MapPath(path);

                if (!Directory.Exists(path))
                    continue;

                foreach (var file in Directory.EnumerateFiles(path, "*.html", SearchOption.AllDirectories))
                {
                    var key = GetKey(file);
                    if (key == null)
                        continue;

                    var script = new TemplateScript(key, () => File.ReadAllText(file));
                    DynamicScriptManager.Register(script);
                    scriptByKey[key.ToLowerInvariant()] = script;
                    bundleList.Add(script.GetScript);
                }

                if (watchForChanges)
                    WatchForChanges(rootUrl);
            }

            bundle = new ConcatenatedScript(bundleList);
            DynamicScriptManager.Register("TemplateBundle", bundle);
        }
    }
}