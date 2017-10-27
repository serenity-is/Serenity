using Serenity.ComponentModel;
using Serenity.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace Serenity.Web
{
    public static class CssBundleManager
    {
        [SettingKey("CssBundling"), SettingScope("Application")]
        private class CssBundlingSettings
        {
            public bool? Enabled { get; set; }
            public bool? Minimize { get; set; }
            public bool? UseMinCSS { get; set; }
        }

        private static bool isEnabled;
        private static bool isInitialized;
        private static Dictionary<string, string[]> cssBundles;
        private static Dictionary<string, string> bundleKeyBySourceUrl;
        private static Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
        private static Dictionary<string, ConcatenatedScript> bundleByKey;
        private static Dictionary<string, List<string>> bundleIncludes;
        private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";

        [ThreadStatic]
        private static HashSet<string> recursionCheck;

        private static Dictionary<string, string[]> CssBundles
        {
            get
            {
                if (cssBundles == null)
                {
                    cssBundles = JsonConfigHelper.LoadConfig<Dictionary<string, string[]>>(
                        HostingEnvironment.MapPath("~/Content/site/CssBundles.json"));
                }

                return cssBundles;
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

        static CssBundleManager()
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
                var settings = Config.Get<CssBundlingSettings>();

                var bundles = CssBundles;
                if (bundles == null ||
                    bundles.Count == 0)
                {
                    return;
                }

                bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://CssBundle.", "css");

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
                    var bundleName = "CssBundle." + bundleKey;
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

                                    if (!scriptName.StartsWith("CssBundle.", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (minimize)
                                        {
                                            try
                                            {
                                                var result = NUglify.Uglify.Css(code);
                                                if (!result.HasErrors)
                                                    code = result.Code;
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Log();
                                            }
                                        }

                                        var scriptUrl = VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + scriptName);
                                        code = RewriteUrlsToAbsolute(scriptUrl, code);
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

                            string code = null;

                            if (minimize)
                            {
                                if (settings.UseMinCSS == true)
                                {
                                    var minPath = Path.ChangeExtension(sourcePath, ".min.css");
                                    if (File.Exists(minPath))
                                    {
                                        using (StreamReader sr = new StreamReader(File.OpenRead(minPath)))
                                            code = sr.ReadToEnd();
                                    }
                                }

                                if (code == null)
                                {
                                    using (StreamReader sr = new StreamReader(File.OpenRead(sourcePath)))
                                        code = sr.ReadToEnd();
                                }

                                try
                                {
                                    var result = NUglify.Uglify.Css(code);
                                    if (!result.HasErrors)
                                        code = result.Code;
                                }
                                catch (Exception ex)
                                {
                                    ex.Log();
                                }
                            }

                            if (code == null)
                            {
                                using (StreamReader sr = new StreamReader(File.OpenRead(sourcePath)))
                                    code = sr.ReadToEnd();
                            }

                            code = RewriteUrlsToAbsolute(sourceUrl, code);
                            return code;
                        });
                    }

                    var bundle = new ConcatenatedScript(bundleParts, "\n\n", checkRights: () =>
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

        public static void CssChanged()
        {
            BundleUtils.ClearVersionCache();
            cssBundles = null;

            if (isEnabled && bundleByKey != null)
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

        private static string UrlToAbsolute(string absolutePath, string url, string prefix, string suffix)
        {
            if (string.IsNullOrWhiteSpace(url) ||
                (url.IndexOf("://") >= 0))
                    return url;

            url = url.TrimStart();
            if (string.IsNullOrWhiteSpace(url) || url[0] == '/')
                return url;

            if (url.StartsWith("data:"))
                return prefix + url + suffix;

            var question = url.IndexOf('?');
            if (question >= 0)
            {
                return new Uri("x:" + absolutePath + url.Substring(0, question)).AbsolutePath.Substring(2)
                    + url.Substring(question);
            }
            
            return new Uri("x:" + absolutePath + url).AbsolutePath.Substring(2);
        }

        private static string RewriteUrlsToAbsolute(string virtualPath, string content)
        {
            if (string.IsNullOrEmpty(content) || 
                string.IsNullOrEmpty(virtualPath))
                return content;

            var absolutePath = Path.GetDirectoryName(virtualPath)
                .Replace('\\', '/');

            if (string.IsNullOrWhiteSpace(absolutePath))
                return content;

            if (absolutePath.StartsWith("~"))
                absolutePath = VirtualPathUtility.ToAbsolute(absolutePath);

            if (!absolutePath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                absolutePath += "/";

            var regex = new Regex("url\\((?<prefix>['\"]?)(?<url>[^)]+?)(?<suffix>['\"]?)\\)");
            return regex.Replace(content, (Match match) => "url(" + 
                UrlToAbsolute(absolutePath, match.Groups["url"].Value, 
                    match.Groups["prefix"].Value, 
                    match.Groups["suffix"].Value) + ")");
        }

        public static string GetCssBundle(string cssUrl)
        {
            Initialize();

            string bundleKey;

            if (cssUrl != null && cssUrl.StartsWith("dynamic://",
                StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = cssUrl.Substring(10);
                if (!isEnabled || !bundleKeyBySourceUrl.TryGetValue(cssUrl, out bundleKey))
                {
                    cssUrl = DynamicScriptManager.GetScriptInclude(scriptName, ".css");
                    return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + cssUrl);
                }
            }
            else
            {
                cssUrl = BundleUtils.ExpandVersionVariable(cssUrl);
                cssUrl = VirtualPathUtility.ToAbsolute(cssUrl);

                if (!isEnabled ||
                    bundleKeyBySourceUrl == null ||
                    !bundleKeyBySourceUrl.TryGetValue(cssUrl, out bundleKey))
                    return cssUrl;
            }

            string include = DynamicScriptManager.GetScriptInclude("CssBundle." + bundleKey, ".css");
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}