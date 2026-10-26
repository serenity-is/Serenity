#if IsTemplateBuild
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    private static IReadOnlyDictionary<string, string> templateProperties;

    /// <summary>
    /// Evaluated MSBuild properties from Package.Version.props (imported by the
    /// template csproj inside its IsNotTemplate block). The template build must
    /// never rewrite the csproj; it only resolves these values into the generated
    /// project and the VSIX manifest.
    /// </summary>
    static IReadOnlyDictionary<string, string> GetTemplateProperties()
    {
        if (templateProperties is not null)
            return templateProperties;

        var names = new[]
        {
            "SerenityVersion",
            "CFPackageVersion",
            "ProPackageVersion",
            "BusinessPackageVersion",
            "EnterprisePackageVersion",
            "SereneTemplateVersion",
            "StartSharpTemplateVersion"
        };

        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = ProjectFolder,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(ProjectFile);
        foreach (var name in names)
            startInfo.ArgumentList.Add("-getProperty:" + name);

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            ExitWithError("Could not start MSBuild to read Package.Version.props.");
            return properties;
        }

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            ExitWithError("Error while reading Package.Version.props from " + ProjectFile);
            return properties;
        }

        using var json = JsonDocument.Parse(output);
        if (!json.RootElement.TryGetProperty("Properties", out var props))
            return properties;

        foreach (var name in names)
        {
            if (props.TryGetProperty(name, out var element))
            {
                var value = element.GetString();
                if (!string.IsNullOrEmpty(value))
                    properties[name] = value;
            }
        }

        templateProperties = properties;
        return properties;
    }

    static string ResolveVersion(string value, IReadOnlyDictionary<string, string> properties)
    {
        if (string.IsNullOrEmpty(value) || properties is null)
            return value;

        foreach (var pair in properties)
            value = value.Replace("$(" + pair.Key + ")", pair.Value, StringComparison.OrdinalIgnoreCase);

        if (value.Contains("$("))
        {
            ExitWithError("Couldn't resolve package version expression '" + value + "' in " + ProjectFile + ".");
        }

        return value;
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

    static List<Tuple<string, string>> ParsePackages(string path,
        IReadOnlyDictionary<string, string> packageVersions = null,
        IReadOnlyDictionary<string, string> templateProperties = null)
    {
        var xml = XElement.Parse(File.ReadAllText(path));
        var pkg = new List<Tuple<string, string>>();
        foreach (var x in xml.Descendants("PackageReference"))
        {
            var packageId = x.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(packageId))
                continue;

            var version = ResolveVersion(x.Attribute("VersionOverride")?.Value, templateProperties);
            if (string.IsNullOrEmpty(version))
                version = ResolveVersion(x.Attribute("Version")?.Value, templateProperties);
            if (string.IsNullOrEmpty(version) && packageVersions != null)
                packageVersions.TryGetValue(packageId, out version);

            if (!string.IsNullOrEmpty(version))
                pkg.Add(new Tuple<string, string>(packageId, version));
        }
        return pkg;
    }

    static void NormalizePackageReferences(XElement projectXml,
        IReadOnlyDictionary<string, string> packageVersions,
        IReadOnlyDictionary<string, string> templateProperties = null)
    {
        foreach (var packageReference in projectXml.Descendants("PackageReference"))
        {
            var packageId = packageReference.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(packageId))
                continue;

            var version = ResolveVersion(packageReference.Attribute("VersionOverride")?.Value, templateProperties);
            if (string.IsNullOrEmpty(version))
                version = ResolveVersion(packageReference.Attribute("Version")?.Value, templateProperties);
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
