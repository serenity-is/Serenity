using Serenity.ComponentModel;
using Serenity.Configuration;
using Serenity.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Serenity.Web
{
    public static class ScriptBundleManager
    {
        [SettingKey("ScriptBundling"), SettingScope("Application")]
        private class ScriptBundlingSettings
        {
            public bool? Enabled { get; set; }
            public bool? Minimize { get; set; }
            public bool? UseMinJS { get; set; }
        }

        private static bool isEnabled;
        private static bool isInitialized;
        private static Dictionary<string, string[]> scriptBundles;
        private static Dictionary<string, string> bundleKeyBySourceUrl;
        private static Dictionary<string, ConcatenatedScript> bundleByKey;
        private static ConcurrentDictionary<string, string> expandVersion;
        private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";

        static ScriptBundleManager()
        {
            expandVersion = new ConcurrentDictionary<string, string>();
        }

        public static Dictionary<string, string[]> ScriptBundles
        {
            get
            {
                if (scriptBundles == null)
                {
                    scriptBundles = JsonConfigHelper.LoadConfig<Dictionary<string, string[]>>(
                        HostingEnvironment.MapPath("~/Scripts/site/ScriptBundles.json"));
                }

                return scriptBundles;
            }
        }

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
                var settings = Config.Get<ScriptBundlingSettings>();
                if (settings.Enabled != true)
                    return;

                var bundles = ScriptBundles;

                if (bundles == null ||
                    bundles.Count == 0)
                {
                    return;
                }

                var bundleKeyBySourceUrlNew = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var bundleByKeyNew = new Dictionary<string, ConcatenatedScript>(StringComparer.OrdinalIgnoreCase);
                bool minimize = settings.Minimize == true;

                foreach (var pair in bundles)
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

                        string sourceUrl = ExpandVersionVariable(sourceFile);
                        sourceUrl = VirtualPathUtility.ToAbsolute(sourceUrl);

                        if (sourceUrl.IsNullOrEmpty())
                            continue;

                        bundleKeyBySourceUrlNew[sourceUrl] = bundleKey;

                        bundleParts.Add(() =>
                        {
                            var sourcePath = HostingEnvironment.MapPath(sourceUrl);
                            if (!File.Exists(sourcePath))
                                return String.Format(errorLines, String.Format("File {0} is not found!", sourcePath));

                            if (minimize)
                            {
                                if (settings.UseMinJS == true)
                                {
                                    var minPath = Path.ChangeExtension(sourcePath, ".min.js");
                                    if (File.Exists(minPath))
                                    {
                                        sourcePath = minPath;
                                        using (StreamReader sr = new StreamReader(File.OpenRead(sourcePath)))
                                            return sr.ReadToEnd();
                                    }
                                }

                                string code;
                                using (StreamReader sr = new StreamReader(File.OpenRead(sourcePath)))
                                    code = sr.ReadToEnd();

                                try
                                {
                                    var result = NUglify.Uglify.Js(code);
                                    if (result.HasErrors)
                                        return code;
                                    return result.Code;
                                }
                                catch (Exception ex)
                                {
                                    ex.Log();
                                    return code;
                                }
                            }

                            using (StreamReader sr = new StreamReader(File.OpenRead(sourcePath)))
                                return sr.ReadToEnd();
                        });
                    }

                    var bundle = new ConcatenatedScript(bundleParts);
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
            expandVersion.Clear();
            scriptBundles = null;

            if (isEnabled && bundleByKey != null)
            {
                foreach (var bundle in bundleByKey.Values)
                    bundle.Changed();

                Reset();
            }
        }

        public static string GetLatestVersion(string path, string mask)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (mask.IsNullOrEmpty())
                return null;

            var idx = mask.IndexOf("*");
            if (idx <= 0)
                throw new ArgumentOutOfRangeException("mask");

            var before = mask.Substring(0, idx);
            var after = mask.Substring(idx + 1);
            var extension = Path.GetExtension(mask);

            var files = Directory.GetFiles(path, mask)
                .Select(x =>
                {
                    var filename = Path.GetFileName(x);
                    return filename.Substring(before.Length, filename.Length - before.Length - after.Length);
                })
                .Where(s =>
                {
                    if (s.Length < 0)
                        return false;
                    int y;
                    return s.Split('.').All(x => Int32.TryParse(x, out y));
                })
                .ToArray();

            if (!files.Any())
                return null;

            Array.Sort(files, (x, y) =>
            {
                var px = x.Split('.');
                var py = y.Split('.');

                for (var i = 0; i < Math.Min(px.Length, py.Length); i++)
                {
                    var c = Int32.Parse(px[i]).CompareTo(Int32.Parse(py[i]));
                    if (c != 0)
                        return c;
                }

                return px.Length.CompareTo(py.Length);
            });

            return files.Last();
        }

        public static string ExpandVersionVariable(string scriptUrl)
        {
            if (scriptUrl.IsNullOrEmpty())
                return scriptUrl;

            var tpl = "{version}";
            var idx = scriptUrl.IndexOf(tpl, StringComparison.OrdinalIgnoreCase);

            if (idx < 0)
                return scriptUrl;
            string result;
            if (expandVersion.TryGetValue(scriptUrl, out result))
                return result;

            var before = scriptUrl.Substring(0, idx);
            var after = scriptUrl.Substring(idx + tpl.Length);
            var extension = Path.GetExtension(scriptUrl);

            var path = HostingEnvironment.MapPath(before);

            path = Path.GetDirectoryName(path);


            var beforeName = Path.GetFileName(before.Replace('/', System.IO.Path.DirectorySeparatorChar));

            var latest = GetLatestVersion(path, beforeName + "*" + extension.Replace('/', System.IO.Path.DirectorySeparatorChar));
            if (latest == null)
            {
                expandVersion[scriptUrl] = scriptUrl;
                return scriptUrl;
            }
            
            result = before + latest + after;
            expandVersion[scriptUrl] = result;
            return result;
        }

        public static string GetScriptBundle(string scriptUrl)
        {
            scriptUrl = ExpandVersionVariable(scriptUrl);
            scriptUrl = VirtualPathUtility.ToAbsolute(scriptUrl);
            Initialize();

            if (!isEnabled || bundleKeyBySourceUrl == null)
                return scriptUrl;
            string bundleKey;
            if (!bundleKeyBySourceUrl.TryGetValue(scriptUrl, out bundleKey))
                return scriptUrl;

            string include = DynamicScriptManager.GetScriptInclude("Bundle." + bundleKey);
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}
