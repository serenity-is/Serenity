using Serenity.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

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
                                throw new InvalidOperationException(String.Format(
                                    "Caught infinite recursion with {1} bundles '{0}'!",
                                        String.Join(", ", recursionCheck), bundleType));
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