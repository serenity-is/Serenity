using Serenity.Web.MvcFakes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Serenity.Web
{
    public class TemplateScriptRegistrar
    {
        const string TemplateSuffix = ".Template.html";
        
        private Dictionary<string, TemplateScript> scriptByKey = new Dictionary<string, TemplateScript>(StringComparer.OrdinalIgnoreCase);

        private static string GetKey(string filename)
        {
            string key = Path.GetFileName(filename);

            if (!key.EndsWith(TemplateSuffix, StringComparison.OrdinalIgnoreCase))
                return null;

            return key.Substring(0, key.Length - TemplateSuffix.Length);
        }

        private void WatchForChanges(string rootUrl)
        {
            var sw = new FileSystemWatcher(HttpContext.Current.Server.MapPath(rootUrl));
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
        }

        public void Initialize(string rootUrl, bool watchForChanges = true)
        {
            var rootPath = HttpContext.Current.Server.MapPath(rootUrl);

            if (!Directory.Exists(rootPath))
                return;

            foreach (var file in Directory.EnumerateFiles(rootPath, "*.html", SearchOption.AllDirectories))
            {
                var key = GetKey(file);
                if (key == null)
                    continue;

                var path = rootUrl + UploadHelper.ToUrl(file.Substring(rootPath.Length));

                scriptByKey[key.ToLowerInvariant()] = new TemplateScript(key, path); // auto registers
            }

            if (watchForChanges)
                WatchForChanges(rootUrl);
        }
    }
}