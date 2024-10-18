#if IsTemplateBuild
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

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

            static string patchVersion(string package, string dependency = null)
            {
                var json = JObject.Parse(File.ReadAllText(Path.Combine(Root, "Serenity", "packages", package, "package.json")));
                return (dependency == null ? json?["version"] : json["dependencies"]?[dependency])?.Value<string>();
            }

            devDependencies["@serenity-is/tsbuild"] = IsPatch ? patchVersion("tsbuild") : 
                GetLatestNpmPackageVersion("@serenity-is/tsbuild");

            File.WriteAllText(PackageJsonFile, root.ToString().Replace("\r", ""));

            foreach (var property in dependencies.Properties().ToList())
            {
                if (!property.Name.StartsWith("@serenity-is/") ||
                    (property.Value.Value<string>()?.StartsWith("file:") != true &&
                     property.Value.Value<string>()?.StartsWith("../") != true))
                    continue;

                dependencies[property.Name] = "./node_modules/.dotnet/" + GetPossibleNuGetPackageId(property.Name);
            }

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

            string toNamespace(string src)
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
    }
}
#endif