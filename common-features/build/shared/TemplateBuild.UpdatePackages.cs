#if IsTemplateBuild
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    private static bool PatchPackageVersion(string packageId, string version)
    {
        var projectContent = File.ReadAllText(ProjectFile);

        var replacedContent = Regex.Replace(projectContent,
            @"(PackageReference\s*Include=\""" + packageId.Replace(".", @"\.") + 
                @"\""\s*Version\s*\=\s*\"")([0-9.]*)(\"")",
            "${1}" + version + "$3");

        if (replacedContent != projectContent)
        {
            File.WriteAllText(ProjectFile, replacedContent);
            return true;
        }

        return false;
    }

    static IEnumerable<string> SerenityPackagesWithSameVersion
    {
        get
        {
            yield return "Serenity.Net.Web";
            yield return "Serenity.Corelib";
            yield return "Serenity.Assets";
        }
    }

    static IEnumerable<string> SerenityPackagesWithUniqueVersion
    {
        get
        {
            yield break;
        }
    }

    static void UpdateSerenityPackages()
    {
        string serenityVersion;
        if (IsPatch)
        {
            var xes = XElement.Parse(File.ReadAllText(SerenityPackageBuildProps));
            serenityVersion = xes.Descendants("Version").FirstOrDefault()?.Value?.ToString();
        }
        else
            serenityVersion = GetLatestVersionOf("Serenity.Net.Web")?.ToString();

        if (!string.IsNullOrEmpty(serenityVersion))
        {
            foreach (var package in SerenityPackagesWithSameVersion)
                PatchPackageVersion(package, serenityVersion);
        }

        foreach (var package in SerenityPackagesWithUniqueVersion)
        {
            var pkgVer = IsPatch ? serenityVersion : GetLatestVersionOf(package)?.ToString();
            if (pkgVer != null)
                PatchPackageVersion(package, pkgVer);
        }
    }

    static bool IsCommonPackage(string packageId)
    {
        return packageId.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) &&
            !IsProPackage(packageId) &&
            (string.Equals(packageId, "Serenity.Extensions", StringComparison.OrdinalIgnoreCase) ||
             packageId.StartsWith("Serenity.Common", StringComparison.OrdinalIgnoreCase) ||
             packageId.StartsWith("Serenity.Demo", StringComparison.OrdinalIgnoreCase));
    }

    static void UpdateCommonAndProPackages()
    {
        string cfPackageVersion = null;
        string proPackageVersion = null;
        string bizPackageVersion = null;
        string entPackageVersion = null;

        if (IsPatch)
        {
            var propsFile = Path.Combine(Root, "common-features", "build", "Package.Build.props");
            var propsRoot = XElement.Parse(File.ReadAllText(propsFile));
            cfPackageVersion = propsRoot.Descendants("Version").FirstOrDefault()?.Value;

            propsFile = Path.Combine(Root, "pro-features", "build", "Package.Build.props");
            if (File.Exists(propsFile))
            {
                propsRoot = XElement.Parse(File.ReadAllText(propsFile));
                proPackageVersion = propsRoot.Descendants("Version").FirstOrDefault()?.Value;
            }

            propsFile = Path.Combine(Root, "business-features", "build", "Package.Build.props");
            if (File.Exists(propsFile))
            {
                propsRoot = XElement.Parse(File.ReadAllText(propsFile));
                bizPackageVersion = propsRoot.Descendants("Version").FirstOrDefault()?.Value;
            }

            propsFile = Path.Combine(Root, "enterprise-features", "build", "Package.Build.props");
            if (File.Exists(propsFile))
            {
                propsRoot = XElement.Parse(File.ReadAllText(propsFile));
                entPackageVersion = propsRoot.Descendants("Version").FirstOrDefault()?.Value;
            }
        }

        string getPackageVersion(string package)
        {
            if (!IsPatch)
                return GetLatestVersionOf(package)?.ToString();

            if (IsCommonPackage(package))
                return cfPackageVersion;

            if (File.Exists(Path.Combine(Root, "business-features", "src", package, package + ".csproj")))
                return bizPackageVersion;

            if (File.Exists(Path.Combine(Root, "enterprise-features", "src", package, package + ".csproj")))
                return entPackageVersion;

            return proPackageVersion;
        }

        var packages = ParsePackages(ProjectFile);
        foreach (var package in packages)
        {
            string packageId = package.Item1;
            if (!IsCommonPackage(packageId) &&
                !IsProPackage(packageId))
                continue;

            string version = getPackageVersion(packageId);
            if (!string.IsNullOrEmpty(version))
                PatchPackageVersion(packageId, version);
        }
    }

    static List<Tuple<string, string>> ParsePackages(string path)
    {
        var xml = XElement.Parse(File.ReadAllText(path));
        var pkg = new List<Tuple<string, string>>();
        foreach (var x in xml.Descendants("PackageReference"))
            pkg.Add(new Tuple<string, string>(x.Attribute("Include").Value, x.Attribute("Version").Value));
        return pkg;
    }
}
#endif