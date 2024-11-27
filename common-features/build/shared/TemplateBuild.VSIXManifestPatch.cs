#if IsTemplateBuild
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Build;

public static partial class Shared
{
    static string PatchVSIXManifest(List<Tuple<string, string>> packages, out string serenityVersion)
    {
        var hash = new HashSet<Tuple<string, string>>();
        foreach (var x in packages)
            hash.Add(x);

        var allPackages = new List<Tuple<string, string>>();
        allPackages.AddRange(hash);
        allPackages.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        if (StartProcess("git", "restore " + VSIXManifestFile, Root) != 0)
            ExitWithError("Error while restoring " + VSIXManifestFile);

        var xm = XElement.Parse(File.ReadAllText(VSIXManifestFile));
        serenityVersion = allPackages.First(x => x.Item1.StartsWith("Serenity.Net")).Item2;
        var identity = xm.Descendants(((XNamespace)"http://schemas.microsoft.com/developer/vsx-schema/2011") + "Identity").First();

        if (TemplateVersion != null &&
            !NuGet.Versioning.NuGetVersion.TryParse(TemplateVersion, out _))
        {
            ExitWithError("Can't parse Template version: " + TemplateVersion);
            return null;
        }

        if (string.IsNullOrEmpty(TemplateVersion))
        {
            var old = identity.Attribute("Version").Value;
            if (old != null && old.StartsWith(serenityVersion + "."))
                TemplateVersion = serenityVersion + "." + (int.Parse(old[(serenityVersion.Length + 1)..]) + 1);
            else
                TemplateVersion = serenityVersion + ".0";
        }

        identity.SetAttributeValue("Version", TemplateVersion);
        File.WriteAllText(VSIXManifestFile, xm.ToString(SaveOptions.OmitDuplicateNamespaces));
        return TemplateVersion;
    }

    static void SetInitialVersionInSergenJson(string templateVersion)
    {
        var root = JObject.Parse(File.ReadAllText(SergenJsonFile));
        if (root["UpgradeInfo"] is not JObject upgradeInfo)
        {
            upgradeInfo = [];
            root["UpgradeInfo"] = upgradeInfo;
            upgradeInfo["InitialType"] = IsStartSharp ? "Premium" : "Community";
        }

        upgradeInfo["InitialVersion"] = templateVersion;
        File.WriteAllText(SergenJsonFile, root.ToString());
    }

    static void SetTemplatesPackageVersion(string templateVersion)
    {
        var xm = XElement.Parse(File.ReadAllText(TemplatesProject));
        var packageVersion = xm.Descendants("PackageVersion").First();
        if (packageVersion == null)
            ExitWithError("Can't find PackageVersion element in: " + TemplatesProject);

        if (packageVersion.Value == templateVersion)
            return;

        packageVersion.Value = templateVersion;
        File.WriteAllText(TemplatesProject, xm.ToString(SaveOptions.OmitDuplicateNamespaces));
    }
}
#endif