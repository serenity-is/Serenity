using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class ProjectFileHelper
    {
        private static string TargetFrameworkExtractor(XElement xe)
        {
            var xtarget = xe.Descendants("TargetFramework").FirstOrDefault();

            if (xtarget == null || string.IsNullOrEmpty(xtarget.Value))
            {
                xtarget = xe.Descendants("TargetFrameworks").FirstOrDefault();
                if (xtarget == null ||
                    string.IsNullOrEmpty(xtarget.Value) &&
                    xtarget.Value.Contains(";", StringComparison.OrdinalIgnoreCase))
                    return null;
            }

            return xtarget?.Value.TrimToNull();
        }

        public static string ExtractTargetFrameworkFrom(string csproj)
        {
            return ExtractPropertyFrom(csproj, TargetFrameworkExtractor);
        }

        public static string ExtractAssemblyNameFrom(string csproj)
        {
            return ExtractPropertyFrom(csproj, xe => 
                xe.Descendants("AssemblyName")
                    .FirstOrDefault()?.Value.TrimToNull());
        }

        public static string ExtractPropertyFrom(string csproj, Func<XElement, string> extractor)
        {
            foreach (var xe in EnumerateProjectAndDirectoryBuildProps(csproj))
            {
                var value = extractor(xe);
                if (value != null)
                    return value;
            }

            return null;
        }

        public static IEnumerable<XElement> EnumerateProjectAndDirectoryBuildProps(string csproj)
        {
            var xe = XElement.Parse(File.ReadAllText(csproj));
            yield return xe;

            var dir = Path.GetDirectoryName(csproj);
            while (!string.IsNullOrEmpty(dir) &&
                Directory.Exists(dir))
            {
                dir = Path.GetFullPath(dir);
                var dirProps = Path.Combine(dir, "Directory.Build.props");
                if (File.Exists(dirProps))
                    yield return XElement.Parse(File.ReadAllText(dirProps));

                dir = Path.GetDirectoryName(dir);
            }
        }
    }
}