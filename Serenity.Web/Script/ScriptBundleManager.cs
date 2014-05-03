using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Serenity.Web;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Web
{
    public static class ScriptBundleManager
    {
        private class ScriptBundlingSettings
        {
            public bool? Enabled { get; set; }
            public bool? Minimize { get; set; }
            public Dictionary<string, string[]> Bundles { get; set; }
        }

        private static bool isEnabled;
        private static bool isInitialized;
        private static Dictionary<string, string> bundleKeyBySourceUrl;
        private static Dictionary<string, ConcatenatedScript> bundleByKey;
        private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";

        public static bool IsEnabled
        {
            get
            {
                Initialize();
                return isEnabled;
            }
        }

        public static void Reset()
        {
            isInitialized = false;
        }

        public static void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;
            isEnabled = false;
            bundleKeyBySourceUrl = null;
            bundleByKey = null;
            try
            {
                var settings = JsonConvert.DeserializeObject<ScriptBundlingSettings>(
                    ConfigurationManager.AppSettings["ScriptBundling"].TrimToNull() ?? "{}", JsonSettings.Tolerant);

                if (settings == null ||
                    settings.Enabled != true)
                    return;

                if (settings.Bundles == null ||
                    settings.Bundles.Count == 0)
                {
                    settings.Bundles = JsConfigHelper.LoadConfig<Dictionary<string, string[]>>(
                        "~/Scripts/Site/ScriptBundles.js");

                    if (settings.Bundles == null ||
                        settings.Bundles.Count == 0)
                        return;
                }

                var bundleKeyBySourceUrlNew = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var bundleByKeyNew = new Dictionary<string, ConcatenatedScript>(StringComparer.OrdinalIgnoreCase);
                bool minimize = settings.Minimize == true;

                foreach (var pair in settings.Bundles)
                {
                    var sourceFiles = pair.Value;
                    if (sourceFiles == null ||
                        sourceFiles.Length == 0)
                        continue;

                    var bundleKey = pair.Key;
                    var bundleName = "Bundle." + bundleKey;
                    var bundleParts = new List<Func<string>>();

                    foreach (var sourceFile in sourceFiles)
                    {
                        if (sourceFile.IsNullOrEmpty())
                            continue;

                        string sourceUrl = 
                            UrlHelper.ResolveUrl(sourceFile);
                        if (sourceUrl.IsNullOrEmpty())
                            continue;

                        bundleKeyBySourceUrlNew[sourceUrl] = bundleKey;

                        bundleParts.Add(() =>
                        {
                            if (HttpContext.Current == null)
                                return String.Format(errorLines, "Tried to generate script while HttpContext is null");

                            var sourcePath = HttpContext.Current.Server.MapPath(sourceUrl);
                            if (!File.Exists(sourcePath))
                                return String.Format(errorLines, String.Format("File {0} is not found!", sourcePath));

                            if (minimize)
                            {
                                var minPath = Path.ChangeExtension(sourcePath, ".min.js");
                                if (File.Exists(minPath))
                                    sourcePath = minPath;
                            }

                            using (StreamReader sr = new StreamReader(sourcePath))
                                return sr.ReadToEnd();
                        });
                    }

                    var bundle = new ConcatenatedScript(bundleParts);
                    bundle.Minimize = settings.Minimize == true;
                    DynamicScriptManager.Register(bundleName, bundle);
                    bundleByKeyNew[bundleKey] = bundle;
                }

                bundleKeyBySourceUrl = bundleKeyBySourceUrlNew;
                bundleByKey = bundleByKeyNew;
                isEnabled = true;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        public static void ScriptsChanged()
        {
            if (isEnabled && bundleByKey != null)
            {
                foreach (var bundle in bundleByKey.Values)
                    bundle.Changed();

                Reset();
            }
        }

        public static string GetScriptBundle(string scriptUrl)
        {
            if (scriptUrl.IsNullOrEmpty())
                return scriptUrl;

            scriptUrl = UrlHelper.ResolveUrl(scriptUrl);

            Initialize();

            if (!isEnabled || bundleKeyBySourceUrl == null)
                return scriptUrl;

            string bundleKey;
            if (!bundleKeyBySourceUrl.TryGetValue(scriptUrl, out bundleKey))
                return scriptUrl;

            string include = DynamicScriptManager.GetScriptInclude("Bundle." + bundleKey);
            return UrlHelper.ResolveUrl("~/DynJS.axd/" + include);
        }
    }
}
