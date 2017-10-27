using Serenity.ComponentModel;
using Serenity.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
        private static Dictionary<string, ConcatenatedScript> bundleByKey;
        private static Dictionary<string, List<string>> bundleIncludes;
        private const string errorLines = "\r\n//\r\n//!!!ERROR: {0}!!!\r\n//\r\n";

        [ThreadStatic]
        private static HashSet<string> recursionCheck;

        private static Dictionary<string, string[]> ScriptBundles
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

        static ScriptBundleManager()
        {
            DynamicScriptManager.ScriptChanged += name =>
            {
                HashSet<string> bundleKeys;
                var b = bundleKeysBySourceUrl;
                if (b != null &&
                    b.TryGetValue("dynamic://" + name, out bundleKeys))
                {
                    foreach (var bundleKey in bundleKeys)
                        DynamicScriptManager.Changed("Bundle." + bundleKey);
                }
            };
        }

        public static void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;
            isEnabled = false;
            bundleKeyBySourceUrl = null;
            bundleKeysBySourceUrl = null;
            bundleByKey = null;
            bundleIncludes = null;
            try
            {
                var settings = Config.Get<ScriptBundlingSettings>();

                var bundles = ScriptBundles;
                if (bundles == null ||
                    bundles.Count == 0)
                {
                    return;
                }

                bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://Bundle.", "script");

                if (settings.Enabled != true)
                    return;

                var bundleKeyBySourceUrlNew = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var bundleKeysBySourceUrlNew = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
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
                    var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    Action<string> registerInBundle = delegate (string sourceFile)
                    {
                        if (bundleKey.IndexOf('/') < 0 && !bundleKeyBySourceUrlNew.ContainsKey(sourceFile))
                            bundleKeyBySourceUrlNew[sourceFile] = bundleKey;

                        HashSet<string> bundleKeys;
                        if (!bundleKeysBySourceUrlNew.TryGetValue(sourceFile, out bundleKeys))
                        {
                            bundleKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            bundleKeysBySourceUrlNew[sourceFile] = new HashSet<string>();
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
                    bundleByKeyNew[bundleKey] = bundle;
                }

                bundleKeyBySourceUrl = bundleKeyBySourceUrlNew;
                bundleKeysBySourceUrl = bundleKeysBySourceUrlNew;
                bundleByKey = bundleByKeyNew;
                isEnabled = true;
            }
            catch (Exception)
            {
                isInitialized = false;
                throw;
            }
        }

        public static void ScriptsChanged()
        {
            BundleUtils.ClearVersionCache();
            scriptBundles = null;

            if (isEnabled)
            {
                var s = bundleByKey;
                if (s != null)
                    foreach (var bundle in s.Values)
                        bundle.Changed();

                isInitialized = false;
            }
        }

        public static IEnumerable<string> GetBundleIncludes(string bundleKey)
        {
            Initialize();

            var bi = bundleIncludes;
            List<string> includes;
            if (bi != null && bi.TryGetValue(bundleKey, out includes) && includes != null)
                return includes;

            return new string[0];
        }

        public static string GetScriptBundle(string scriptUrl)
        {
            Initialize();

            string bundleKey;

            if (scriptUrl != null && scriptUrl.StartsWith("dynamic://",
                StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = scriptUrl.Substring(10);
                if (!isEnabled || !bundleKeyBySourceUrl.TryGetValue(scriptUrl, out bundleKey))
                {
                    scriptUrl = DynamicScriptManager.GetScriptInclude(scriptName);
                    return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + scriptUrl);
                }
            }
            else
            {
                scriptUrl = BundleUtils.ExpandVersionVariable(scriptUrl);
                scriptUrl = VirtualPathUtility.ToAbsolute(scriptUrl);

                if (!isEnabled || 
                    bundleKeyBySourceUrl == null ||
                    !bundleKeyBySourceUrl.TryGetValue(scriptUrl, out bundleKey))
                    return scriptUrl;
            }

            string include = DynamicScriptManager.GetScriptInclude("Bundle." + bundleKey);
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}
