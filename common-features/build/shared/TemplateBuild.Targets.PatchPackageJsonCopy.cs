#if IsTemplateBuild
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using YamlDotNet.Serialization;

namespace Build;

public static partial class Shared
{
    public static partial class Targets
    {
        public static void PatchPackageJsonCopy()
        {
            Directory.CreateDirectory(PackagePatchFolder);

            var content = File.ReadAllText(PackageJsonFile);
            var root = JObject.Parse(content);
            var dependencies = (JObject)root["dependencies"];
            var devDependencies = (JObject)root["devDependencies"];
            var scripts = (JObject)root["scripts"];

            static string patchVersion(string package, string dependency = null)
            {
                var json = JObject.Parse(File.ReadAllText(Path.Combine(Root, "Serenity", "packages", package, "package.json")));
                return (dependency == null ? json?["version"] : json["dependencies"]?[dependency])?.Value<string>();
            }

            static bool shouldFixDependencyVersion(string version)
            {
                return
                    version != null &&
                    (version.StartsWith("file:") ||
                     version.StartsWith("../") ||
                     version.StartsWith("workspace:") ||
                     version.StartsWith("catalog:"));
            }

            var tsbuildVer = IsPatch ? patchVersion("tsbuild") : GetLatestNpmPackageVersion("@serenity-is/tsbuild");
            if (!shouldFixDependencyVersion(devDependencies["@serenity-is/tsbuild"]?.Value<string>()))
                devDependencies["@serenity-is/tsbuild"] = tsbuildVer;

            File.WriteAllText(PackageJsonFile, root.ToString().Replace("\r", ""));

            devDependencies["@serenity-is/tsbuild"] = tsbuildVer;
            devDependencies.Remove("test-utils");

            PnpmWorkspaceYaml workspaceYaml = null;

            foreach (var deps in new JObject[] { dependencies, devDependencies })
            {
                foreach (var property in deps.Properties().ToList())
                {
                    string version = property.Value.Value<string>();

                    if (!shouldFixDependencyVersion(version))
                        continue;

                    if (property.Name.StartsWith("@serenity-is/"))
                    {
                        deps[property.Name] = "./node_modules/.dotnet/" + GetPossibleNuGetPackageId(property.Name);
                    }
                    else if (version.StartsWith("catalog:"))
                    {
                        if (workspaceYaml == null)
                        {
                            string workspaceFile = Path.Combine(SerenityFolder, "pnpm-workspace.yaml");
                            if (!File.Exists(workspaceFile))
                            {
                                ExitWithError($"File does not exist: {workspaceFile} !");
                                return;
                            }

                            var yaml = File.ReadAllText(Path.Combine(SerenityFolder, "pnpm-workspace.yaml"));
                            var deserializer = new DeserializerBuilder()
                                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.NullNamingConvention.Instance)
                                .IgnoreUnmatchedProperties()
                                .Build();
                            workspaceYaml = deserializer.Deserialize<PnpmWorkspaceYaml>(yaml);
                        }

                        var catalogName = version["catalog:".Length..];
                        if ((string.IsNullOrEmpty(catalogName) ||
                             workspaceYaml.catalogs?.TryGetValue(catalogName, out var catalog) != true ||
                             catalog?.TryGetValue(property.Name, out var catalogVersion) != true) &&
                             workspaceYaml.catalog?.TryGetValue(property.Name, out catalogVersion) != true)
                        {
                            ExitWithError($"Catalog package version not found: {property.Name} in {version} !");
                            return;
                        }

                        deps[property.Name] = catalogVersion;
                    }
                    else
                    {
                        ExitWithError($"Unsupported package version format: {property.Name} : {version} !");
                        return;
                    }
                }
            }

            scripts.Remove("jest");
            scripts.Remove("test");

            content = root.ToString().Replace("\r", "");
            File.WriteAllText(PackageJsonCopy, content);

            if (File.Exists(PackageJsonCopyLock))
                File.Delete(PackageJsonCopyLock);

            var dotnetDir = Path.Combine(PackagePatchFolder, "node_modules", ".dotnet");
            if (!Directory.Exists(dotnetDir))
            {
                if (!Directory.Exists(Path.GetDirectoryName(dotnetDir)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dotnetDir));
                Directory.CreateSymbolicLink(dotnetDir, Path.Combine(Path.GetDirectoryName(PackageJsonFile), "node_modules", ".dotnet"));
            }

            if (StartProcess("cmd", "/c npm i --ignore-scripts", PackagePatchFolder) != 0)
            {
                Console.Error.WriteLine("Error while npm install at " + PackagePatchFolder);
                Environment.Exit(1);
            }
        }

        private static readonly char[] packageIdSplitChars = ['-', '.', '/'];
        private static string GetPossibleNuGetPackageId(string moduleName)
        {
            moduleName = moduleName.ToLowerInvariant();

            static string toNamespace(string src)
            {
                return string.Join(".", src.Split(packageIdSplitChars,
                    StringSplitOptions.RemoveEmptyEntries));
            }

            var idx = moduleName.IndexOf('/');
            if (idx < 0)
                return toNamespace(moduleName);

            var company = moduleName[1..idx];
            if (company == "serenity-is")
                company = "serenity";
            else if (company.Length > 0)
                company = toNamespace(company);

            return company + "." + toNamespace(moduleName[(idx + 1)..]);
        }

        private class PnpmWorkspaceYaml
        {
            public Dictionary<string, string> catalog { get; set; } = [];
            public Dictionary<string, Dictionary<string, string>> catalogs { get; set; } = [];
        }
    }
}
#endif