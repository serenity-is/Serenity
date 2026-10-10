#if IsTemplateBuild
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    private static bool PatchPackageVersion(string packageId, string version)
    {
        var projectContent = File.ReadAllText(ProjectFile);
        var pattern = $"(PackageReference\\s*Include=\"{Regex.Escape(packageId)}\"\\s*(?:VersionOverride|Version)\\s*=\\s*\")([^\"]*)(\")";
        var replacedContent = Regex.Replace(projectContent, pattern,
            match => match.Groups[1].Value + version + match.Groups[3].Value);

        if (replacedContent != projectContent)
        {
            File.WriteAllText(ProjectFile, replacedContent);
            return true;
        }

        return false;
    }

    static Dictionary<string, string> GetPackageVersions()
    {
        var packageVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = ProjectFolder,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(ProjectFile);
        startInfo.ArgumentList.Add("-getItem:PackageVersion");

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            ExitWithError("Could not start MSBuild to read PackageVersion items.");
            return packageVersions;
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            ExitWithError("Error while reading PackageVersion items from " + ProjectFile);
            return packageVersions;
        }

        using var json = JsonDocument.Parse(output);
        if (!json.RootElement.TryGetProperty("Items", out var items) ||
            !items.TryGetProperty("PackageVersion", out var centralVersions))
            return packageVersions;

        foreach (var item in centralVersions.EnumerateArray())
        {
            if (!item.TryGetProperty("Identity", out var identityElement) ||
                !item.TryGetProperty("Version", out var versionElement))
                continue;

            var packageId = identityElement.GetString();
            var version = versionElement.GetString();
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(version))
                packageVersions[packageId] = version;
        }

        return packageVersions;
    }

    static IEnumerable<string> SerenityPackagesWithSameVersion
    {
        get
        {
            yield return "Serenity.Net.Web";
            yield return "Serenity.Corelib";
            yield return "Serenity.Assets";
            yield return "Serenity.DomWise";
            yield return "Serenity.SleekGrid";
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
            var xes = XElement.Parse(File.ReadAllText(SerenityPackageVersionProps));
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
            var propsFile = Path.Combine(Root, "Serenity", "common-features", "build", "Package.Build.props");
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

    static List<Tuple<string, string>> ParsePackages(string path,
        IReadOnlyDictionary<string, string> packageVersions = null)
    {
        var xml = XElement.Parse(File.ReadAllText(path));
        var pkg = new List<Tuple<string, string>>();
        foreach (var x in xml.Descendants("PackageReference"))
        {
            var packageId = x.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(packageId))
                continue;

            var version = x.Attribute("VersionOverride")?.Value;
            if (string.IsNullOrEmpty(version))
                version = x.Attribute("Version")?.Value;
            if (string.IsNullOrEmpty(version) && packageVersions != null)
                packageVersions.TryGetValue(packageId, out version);

            if (!string.IsNullOrEmpty(version))
                pkg.Add(new Tuple<string, string>(packageId, version));
        }
        return pkg;
    }

    static void NormalizePackageReferences(XElement projectXml,
        IReadOnlyDictionary<string, string> packageVersions)
    {
        foreach (var packageReference in projectXml.Descendants("PackageReference"))
        {
            var packageId = packageReference.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(packageId))
                continue;

            var version = packageReference.Attribute("VersionOverride")?.Value;
            if (string.IsNullOrEmpty(version))
                version = packageReference.Attribute("Version")?.Value;
            if (string.IsNullOrEmpty(version))
                packageVersions.TryGetValue(packageId, out version);

            if (string.IsNullOrEmpty(version))
            {
                ExitWithError($"Couldn't determine package version for '{packageId}' in {ProjectFile}.");
                return;
            }

            packageReference.Attribute("VersionOverride")?.Remove();
            packageReference.SetAttributeValue("Version", version);
        }
    }
}
#endif