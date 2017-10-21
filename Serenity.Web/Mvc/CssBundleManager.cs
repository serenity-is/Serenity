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
        private static Dictionary<string, ConcatenatedScript> bundleByKey;
        private static ConcurrentDictionary<string, string> expandVersion;
        private const string errorLines = "\r\n/*\r\n!!!ERROR: {0}!!!\r\n*/\r\n";

        static CssBundleManager()
        {
            expandVersion = new ConcurrentDictionary<string, string>();
        }

        public static Dictionary<string, string[]> CssBundles
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
                var settings = Config.Get<CssBundlingSettings>();
                if (settings.Enabled != true)
                    return;

                var bundles = CssBundles;

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
                    var bundleName = "CssBundle." + bundleKey;
                    var bundleParts = new List<Func<string>>();

                    foreach (var sourceFile in sourceFiles)
                    {
                        if (sourceFile.IsNullOrEmpty())
                            continue;

                        string sourceUrl = ScriptBundleManager.ExpandVersionVariable(sourceFile);
                        sourceUrl = VirtualPathUtility.ToAbsolute(sourceUrl);

                        if (sourceUrl.IsNullOrEmpty())
                            continue;

                        bundleKeyBySourceUrlNew[sourceUrl] = bundleKey;

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

                    var bundle = new ConcatenatedScript(bundleParts, "\n\n");
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

        public static void CssChanged()
        {
            expandVersion.Clear();
            cssBundles = null;

            if (isEnabled && bundleByKey != null)
            {
                foreach (var bundle in bundleByKey.Values)
                    bundle.Changed();

                Reset();
            }
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
            cssUrl = ScriptBundleManager.ExpandVersionVariable(cssUrl);
            cssUrl = VirtualPathUtility.ToAbsolute(cssUrl);
            Initialize();

            if (!isEnabled || bundleKeyBySourceUrl == null)
                return cssUrl;

            string bundleKey;
            if (!bundleKeyBySourceUrl.TryGetValue(cssUrl, out bundleKey))
                return cssUrl;

            string include = DynamicScriptManager.GetScriptInclude("CssBundle." + bundleKey, ".css");
            return VirtualPathUtility.ToAbsolute("~/DynJS.axd/" + include);
        }
    }
}