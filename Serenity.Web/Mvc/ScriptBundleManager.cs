using Serenity.ComponentModel;
using Serenity.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Serenity.Web
{
    public class ScriptBundleManager
    {
        [SettingKey("ScriptBundling"), SettingScope("Application")]
        private class ScriptBundlingSettings
        {
            public bool? Enabled { get; set; }
            public bool? Minimize { get; set; }
            public bool? UseMinJS { get; set; }
        }

        private static object initializationLock = new object();

        private bool isEnabled;
        private Dictionary<string, string> bundleKeyBySourceUrl;
        private Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
        private Dictionary<string, ConcatenatedScript> bundleByKey;
        private Dictionary<string, List<string>> bundleIncludes;

        private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";

        [ThreadStatic]
        private static HashSet<string> recursionCheck;

        private static ScriptBundleManager instance;

        private static ScriptBundleManager Instance
        {
            get
            {
                var instance = ScriptBundleManager.instance;
                if (instance != null)
                    return instance;

                lock (initializationLock)
                {
                    instance = ScriptBundleManager.instance;
                    if (instance != null)
                        return instance;

                    ScriptBundleManager.instance = instance = new ScriptBundleManager();
                }

                return instance;
            }
        }

        public ScriptBundleManager()
        {
            var settings = Config.Get<ScriptBundlingSettings>();

            var bundles = JsonConfigHelper.LoadConfig<Dictionary<string, string[]>>(
                HostingEnvironment.MapPath("~/Scripts/site/ScriptBundles.json"));

            bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://Bundle.", "script");

            if (bundles.Count == 0 ||
                settings.Enabled != true)
            {
                return;
            }

            bundleKeyBySourceUrl = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bundleKeysBySourceUrl = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            bundleByKey = new Dictionary<string, ConcatenatedScript>(StringComparer.OrdinalIgnoreCase);

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
                var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                Action<string> registerInBundle = delegate (string sourceFile)
                {
                    if (bundleKey.IndexOf('/') < 0 && !bundleKeyBySourceUrl.ContainsKey(sourceFile))
                        bundleKeyBySourceUrl[sourceFile] = bundleKey;

                    HashSet<string> bundleKeys;
                    if (!bundleKeysBySourceUrl.TryGetValue(sourceFile, out bundleKeys))
                    {
                        bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bundleKeysBySourceUrl[sourceFile] = new HashSet<string>();
                    }

                    bundleKeys.Add(bundleKey);
                };

                foreach (var sourceFile in sourceFiles)
                {
                    if (sourceFile.IsNullOrEmpty())
                        continue;

                    if (sourceFile.StartsWith("dynamic://", StringComparison.OrdinalIgnoreCase))
                    {
                        registerInBundle(sourceFile);

                        var scriptName = sourceFile.Substring(10);
                        scriptNames.Add(scriptName);
                        bundleParts.Add(() =>
                        {
                            if (recursionCheck != null)
                            {
                                if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                                    return String.Format(errorLines,
                                        String.Format("Caught infinite recursion with dynamic scripts '{0}'!",
                                            String.Join(", ", recursionCheck)));
                            }
                            else
                                recursionCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            recursionCheck.Add(scriptName);
                            try
                            {
                                var code = DynamicScriptManager.GetScriptText(scriptName);
                                if (code == null)
                                    return String.Format(errorLines,
                                        String.Format("Dynamic script with name '{0}' is not found!", scriptName));

                                if (minimize && !scriptName.StartsWith("Bundle.",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        var result = NUglify.Uglify.Js(code);
                                        if (!result.HasErrors)
                                            code = result.Code;
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Log();
                                    }
                                }

                                return code;
                            }
                            finally
                            {
                                recursionCheck.Remove(scriptName);
                            }
                        });

                        continue;
                    }

                    string sourceUrl = BundleUtils.ExpandVersionVariable(sourceFile);
                    sourceUrl = VirtualPathUtility.ToAbsolute(sourceUrl);

                    if (sourceUrl.IsNullOrEmpty())
                        continue;

                    registerInBundle(sourceUrl);

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

                var bundle = new ConcatenatedScript(bundleParts, checkRights: () =>
                {
                    foreach (var scriptName in scriptNames)
                    {
                        if (recursionCheck != null)
                        {
                            if (recursionCheck.Contains(scriptName) || recursionCheck.Count > 100)
                                throw new InvalidOperationException(String.Format(
                                    "Caught infinite recursion with dynamic scripts '{0}'!",
                                        String.Join(", ", recursionCheck)));
                        }
                        else
                            recursionCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        recursionCheck.Add(scriptName);
                        try
                        {
                            DynamicScriptManager.CheckScriptRights(scriptName);
                        }
                        finally
                        {
                            recursionCheck.Remove(scriptName);
                        }
                    }
                });

                DynamicScriptManager.Register(bundleName, bundle);
                bundleByKey[bundleKey] = bundle;
            }

            isEnabled = true;
        }

        public static bool IsEnabled
        {
            get
            {
                return Instance.isEnabled;
            }
        }

        static ScriptBundleManager()
        {
            DynamicScriptManager.ScriptChanged += name =>
            {
                var instance = ScriptBundleManager.instance;

                HashSet<string> bundleKeys;
                if (instance != null &&
                    instance.bundleKeysBySourceUrl != null &&
                    instance.bundleKeysBySourceUrl.TryGetValue("dynamic://" + name, out bundleKeys))
                {
                    foreach (var bundleKey in bundleKeys)
                        DynamicScriptManager.Changed("Bundle." + bundleKey);
                }
            };
        }

        public static void ScriptsChanged()
        {
            BundleUtils.ClearVersionCache();
            var instance = ScriptBundleManager.instance;

            if (instance != null && 
                instance.bundleByKey != null)
            {
                foreach (var bundle in instance.bundleByKey.Values)
                    bundle.Changed();
            }

            instance = null;
        }

        public static IEnumerable<string> GetBundleIncludes(string bundleKey)
        {
            var bi = Instance.bundleIncludes;
            List<string> includes;
            if (bi != null && bi.TryGetValue(bundleKey, out includes) && includes != null)
                return includes;

            return new string[0];
        }

        public static string GetScriptBundle(string scriptUrl)
        {
            string bundleKey;
            var bySrcUrl = Instance.bundleKeyBySourceUrl;

            if (scriptUrl != null && scriptUrl.StartsWith("dynamic://",
                StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = scriptUrl.Substring(10);
                if (bySrcUrl == null || !bySrcUrl.TryGetValue(scriptUrl, out bundleKey))
                {
                    scriptUrl = DynamicScriptManager.GetScriptInclude(scriptName);
                    return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + scriptUrl);
                }
            }
            else
            {
                scriptUrl = BundleUtils.ExpandVersionVariable(scriptUrl);
                scriptUrl = VirtualPathUtility.ToAbsolute(scriptUrl);

                if (bySrcUrl == null ||
                    !bySrcUrl.TryGetValue(scriptUrl, out bundleKey))
                    return scriptUrl;
            }

            string include = DynamicScriptManager.GetScriptInclude("Bundle." + bundleKey);
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}
