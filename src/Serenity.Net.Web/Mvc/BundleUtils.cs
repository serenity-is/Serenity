using Serenity.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Serenity.Web
{
    internal static class BundleUtils
    {
        internal static ConcurrentDictionary<string, string> expandVersion;

        static BundleUtils()
        {
            expandVersion = new ConcurrentDictionary<string, string>();
        }

        public static void ClearVersionCache()
        {
            expandVersion.Clear();
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
                    return s.Split('.').All(x => int.TryParse(x, out y));
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
                    var c = int.Parse(px[i]).CompareTo(int.Parse(py[i]));
                    if (c != 0)
                        return c;
                }

                return px.Length.CompareTo(py.Length);
            });

            return files.Last();
        }

        public static string ExpandVersionVariable(string webRootPath, string scriptUrl)
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

            var path = PathHelper.SecureCombine(webRootPath, before.StartsWith("~/") ? before[2..] : before);
            path = Path.GetDirectoryName(path);

            var beforeName = Path.GetFileName(before.Replace('/', Path.DirectorySeparatorChar));

            var latest = GetLatestVersion(path, beforeName + "*" + extension.Replace('/', Path.DirectorySeparatorChar));
            if (latest == null)
            {
                expandVersion[scriptUrl] = scriptUrl;
                return scriptUrl;
            }

            result = before + latest + after;
            expandVersion[scriptUrl] = result;
            return result;
        }

        public static string DoReplacements(string scriptUrl, Dictionary<string, object> replacements)
        {
            int idx = 0;
            do
            {
                idx = scriptUrl.IndexOf('{', idx);
                if (idx < 0)
                    break;

                var end = scriptUrl.IndexOf('}', idx + 1);
                if (end < 0)
                    break;

                var key = scriptUrl.Substring(idx + 1, end - idx - 1);
                if (string.IsNullOrEmpty(key))
                    return null;

                if (key == "version")
                {
                    idx = end + 1;
                    continue;
                }

                bool falsey = key.StartsWith("!");
                if (falsey)
                    key = key.Substring(1);

                if (string.IsNullOrEmpty(key))
                    return null;

                object value;
                if (replacements == null ||
                    !replacements.TryGetValue(key, out value))
                {
                    value = null;
                }

                string replace;
                if (falsey)
                {
                    if (value is bool && (bool)value == true)
                        return null;

                    if (value != null && (!(value is bool) || ((bool)value == true)))
                        return null;

                    replace = "";
                }
                else
                {
                    if (value == null)
                        return null;

                    if (value is bool b && b == false)
                        return null;

                    if (value is bool b2 && b2 == true)
                        replace = "";
                    else
                        replace = value.ToString();
                }

                scriptUrl = scriptUrl.Substring(0, idx) + replace + scriptUrl[(end + 1)..];
                idx = end + 1 + (replace.Length - key.Length - (falsey ? 1 : 0));
            }
            while (idx < scriptUrl.Length - 1);

            return scriptUrl;
        }

        public static Dictionary<string, List<string>> ExpandBundleIncludes(Dictionary<string, string[]> bundles,
            string bundlePrefix, string bundleType)
        {
            var recursionCheck = new HashSet<string>();

            var bundleIncludes = new Dictionary<string, List<string>>();
            Func<string, List<string>> listBundleIncludes = null;

            listBundleIncludes = (string bundleKey) =>
            {
                var includes = new List<string>();

                string[] sourceFiles;
                if (!bundles.TryGetValue(bundleKey, out sourceFiles) ||
                    sourceFiles == null ||
                    sourceFiles.Length == 0)
                    return includes;

                foreach (var sourceFile in sourceFiles)
                {
                    if (sourceFile.IsNullOrEmpty())
                        continue;

                    if (sourceFile.StartsWith(bundlePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var subBundleKey = sourceFile.Substring(17);
                        if (recursionCheck != null)
                        {
                            if (recursionCheck.Contains(subBundleKey) || recursionCheck.Count > 100)
                                throw new InvalidOperationException(string.Format(
                                    "Caught infinite recursion with {1} bundles '{0}'!",
                                        string.Join(", ", recursionCheck), bundleType));
                        }
                        else
                            recursionCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        recursionCheck.Add(subBundleKey);
                        try
                        {
                            includes.AddRange(listBundleIncludes(subBundleKey));
                        }
                        finally
                        {
                            recursionCheck.Remove(subBundleKey);
                        }
                    }
                    else
                        includes.Add(sourceFile);
                }

                return includes;
            };

            foreach (var bundleKey in bundles.Keys)
            {
                var includes = listBundleIncludes(bundleKey);
                bundleIncludes[bundleKey] = includes;
            }

            return bundleIncludes;
        }
    }
}