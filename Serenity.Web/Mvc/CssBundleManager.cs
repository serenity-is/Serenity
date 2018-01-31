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
    public class CssBundleManager
    {
        [SettingKey("CssBundling"), SettingScope("Application")]
        private class CssBundlingSettings
        {
            public bool? Enabled { get; set; }
            public bool? Minimize { get; set; }
            public bool? UseMinCSS { get; set; }
        }

        private bool isEnabled;
        private Dictionary<string, string> bundleKeyBySourceUrl;
        private Dictionary<string, HashSet<string>> bundleKeysBySourceUrl;
        private Dictionary<string, ConcatenatedScript> bundleByKey;
        private Dictionary<string, List<string>> bundleIncludes;

        private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";

        [ThreadStatic]
        private static HashSet<string> recursionCheck;

        public static bool IsEnabled
        {
            get
            {
                return Instance.isEnabled;
            }
        }

        private static object initializationLock = new object();
        private static CssBundleManager instance;

        private static CssBundleManager Instance
        {
            get
            {
                var instance = CssBundleManager.instance;
                if (instance != null)
                    return instance;

                lock (initializationLock)
                {
                    instance = CssBundleManager.instance;
                    if (instance != null)
                        return instance;

                    CssBundleManager.instance = instance = new CssBundleManager();
                }

                return instance;
            }
        }

        public CssBundleManager()
        {
            
            var settings = Config.Get<CssBundlingSettings>();

            var bundles = JsonConfigHelper.LoadConfig<Dictionary<string, string[]>>(
                HostingEnvironment.MapPath("~/Content/site/CssBundles.json"));

            bundleIncludes = BundleUtils.ExpandBundleIncludes(bundles, "dynamic://CssBundle.", "css");

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
                var bundleName = "CssBundle." + bundleKey;
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
                bundleByKey[bundleKey] = bundle;
            }

            isEnabled = true;
        }

        static CssBundleManager()
        {
            DynamicScriptManager.ScriptChanged += name =>
            {
                var instance = CssBundleManager.instance;

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

        public static void CssChanged()
        {
            BundleUtils.ClearVersionCache();
            var instance = CssBundleManager.instance;

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
            string bundleKey;
            var bySrcUrl = Instance.bundleKeyBySourceUrl;

            if (cssUrl != null && cssUrl.StartsWith("dynamic://",
                StringComparison.OrdinalIgnoreCase))
            {
                var scriptName = cssUrl.Substring(10);
                if (bySrcUrl == null || !bySrcUrl.TryGetValue(cssUrl, out bundleKey))
                {
                    cssUrl = DynamicScriptManager.GetScriptInclude(scriptName, ".css");
                    return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + cssUrl);
                }
            }
            else
            {
                cssUrl = BundleUtils.ExpandVersionVariable(cssUrl);
                cssUrl = VirtualPathUtility.ToAbsolute(cssUrl);

                if (bySrcUrl == null ||
                    !bySrcUrl.TryGetValue(cssUrl, out bundleKey))
                    return cssUrl;
            }

            string include = DynamicScriptManager.GetScriptInclude("CssBundle." + bundleKey, ".css");
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}