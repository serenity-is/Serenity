using Serenity.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class RestoreCommand
    {
        private static string[] skipPackages = new[]
        {
            "Dapper",
            "EPPlus",
            "FastMember",
            "FluentMigrator.",
            "FirebirdSql.",
            "MailKit",
            "Mapster",
            "MySql",
            "Microsoft.",
            "Newtonsoft.",
            "NetStandard.",
            "Npgsql",
            "Nuglify.",
            "StackExchange.",
            "System.",
            "Serenity.Net."
        };

        public void Run(string csproj)
        {
            var packagesDir = new PackageHelper().DeterminePackagesPath();

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Func<string, bool> skipPackage = id =>
            {
                if (visited.Contains(id))
                    return true;

                foreach (var skip in skipPackages)
                    if (id.StartsWith(skip, StringComparison.OrdinalIgnoreCase))
                        return true;

                visited.Add(id);

                return false;
            };

            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            GlobFilter include = null;
            if (config.Restore?.Include.IsEmptyOrNull() == false)
                include = new GlobFilter(config.Restore.Include);

            GlobFilter exclude = null;
            if (config.Restore?.Exclude.IsEmptyOrNull() == false)
                exclude = new GlobFilter(config.Restore.Exclude);

            var targetRoot = Path.GetDirectoryName(csproj);
            var restoredFromProjectReference = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void restoreFile(string file, string relative)
            {
                relative = PathHelper.ToPath(relative);

                if (include != null &&
                    !include.IsMatch(relative))
                    return;

                if (exclude != null &&
                    exclude.IsMatch(relative))
                    return;

                if (restoredFromProjectReference.Contains(relative))
                    return;

                var target = Path.Combine(targetRoot, relative);
                if (File.Exists(target))
                {
                    if (!File.ReadAllBytes(target)
                            .SequenceEqual(File.ReadAllBytes(file)))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Restoring: " + relative);
                        Console.ResetColor();
                        File.Copy(file, target, true);
                    }
                }
                else
                {
                    if (!Directory.Exists(target))
                        Directory.CreateDirectory(Path.GetDirectoryName(target));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Restoring: " + relative);
                    Console.ResetColor();
                    File.Copy(file, target, false);
                }
            }

            foreach (var reference in EnumerateProjectReferences(csproj, new HashSet<string>(StringComparer.OrdinalIgnoreCase)))
            {
                foreach (var root in ProjectFileHelper.EnumerateProjectAndDirectoryBuildProps(reference))
                {
                    foreach (var itemGroup in root.Elements("ItemGroup"))
                    {
                        foreach (var content in itemGroup.Elements("Content")
                            .Concat(itemGroup.Elements("None")))
                        {
                            var sourceFile = content.Attribute("Include")?.Value?.Trim();
                            if (string.IsNullOrEmpty(sourceFile))
                                continue;

                            sourceFile = sourceFile.Replace("$(ProjectDir)", 
                                Path.GetDirectoryName(reference) + Path.DirectorySeparatorChar,
                                StringComparison.OrdinalIgnoreCase);

                            sourceFile = sourceFile.Replace("$(ProjectName.ToLowerInvariant())",
                                Path.GetFileNameWithoutExtension(reference).ToLowerInvariant(),
                                StringComparison.OrdinalIgnoreCase);

                            sourceFile = sourceFile.Replace("$(ProjectName)",
                                Path.GetFileNameWithoutExtension(reference),
                                StringComparison.OrdinalIgnoreCase);

                            sourceFile = Path.Combine(Path.GetDirectoryName(reference), sourceFile);

                            if (!File.Exists(sourceFile))
                                continue;

                            if (string.Equals(content.Element("Pack")?.Value?.Trim(), "true"))
                            {
                                var packagePath = content.Element("PackagePath")?.Value?.Trim();
                                if (!string.IsNullOrEmpty(packagePath))
                                {
                                    packagePath = packagePath.Replace("$(ProjectName.ToLowerInvariant())",
                                        Path.GetFileNameWithoutExtension(reference).ToLowerInvariant(),
                                        StringComparison.OrdinalIgnoreCase);

                                    packagePath = packagePath.Replace("$(ProjectName)",
                                        Path.GetFileNameWithoutExtension(reference),
                                        StringComparison.OrdinalIgnoreCase);

                                    foreach (var path in packagePath.Split(';', StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (path.Contains('$', StringComparison.Ordinal))
                                            continue;

                                        if (!PathHelper.ToUrl(path).StartsWith("typings/", StringComparison.OrdinalIgnoreCase))
                                            continue;

                                        restoreFile(sourceFile, path);
                                        restoredFromProjectReference.Add(path);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var queue = new Queue<(string Target, string ID, string Version)>();
            foreach (var x in EnumeratePackageReferences(csproj))
            {
                if (!skipPackage(x.Id) && !string.IsNullOrEmpty(x.Version))
                    queue.Enqueue(x);
            };

            while (queue.Count > 0)
            {
                var dep = queue.Dequeue();
                var id = dep.Item2;

                var ver = dep.Item3.Trim();
                if (ver.EndsWith("-*", StringComparison.Ordinal))
                    ver = ver.Substring(0, ver.Length - 2);
                else if (ver.StartsWith("[", StringComparison.Ordinal) && ver.EndsWith("]", StringComparison.Ordinal))
                {
                    ver = ver.Substring(1, ver.Length - 2).Trim();
                }

                var packageFolder = Path.Combine(Path.Combine(packagesDir, id), ver);
                if (!Directory.Exists(packageFolder))
                {
                    packageFolder = Path.Combine(Path.Combine(packagesDir, id.ToLowerInvariant()), ver);
                    if (!Directory.Exists(packageFolder))
                    {
                        var myPackagesDir = Path.Combine(packagesDir, "..", "my-packages");
                        packageFolder = Path.Combine(myPackagesDir, id, ver);
                        if (!Directory.Exists(packageFolder))
                            packageFolder = Path.Combine(myPackagesDir, id.ToLowerInvariant(), ver);
                    }
                }

                var nuspecFile = Path.Combine(packageFolder, id + ".nuspec");
                if (!File.Exists(nuspecFile))
                {
                    nuspecFile = Path.Combine(packageFolder, id.ToLowerInvariant() + ".nuspec");
                    if (!File.Exists(nuspecFile))
                        continue;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Processing: " + id);
                Console.ResetColor();

                var contentRoot = Path.Combine(packageFolder, "content");
                if (!Directory.Exists(contentRoot))
                    contentRoot = Path.Combine(packageFolder, "Content");

                if (Directory.Exists(contentRoot))
                {
                    foreach (var file in Directory.GetFiles(contentRoot, "*.*", SearchOption.AllDirectories))
                    {
                        var extension = Path.GetExtension(file);
                        if (string.Compare(extension, ".transform", StringComparison.OrdinalIgnoreCase) == 0)
                            continue;

                        var relative = PathHelper.ToUrl(file.Substring(contentRoot.Length + 1));

                        // normalize paths as Content, Scripts, fonts and typings (these are exact cases expected)
                        if (relative.StartsWith("content/", StringComparison.OrdinalIgnoreCase))
                            relative = "Content/" + relative.Substring("content/".Length);
                        else if (relative.StartsWith("scripts/typings/", StringComparison.OrdinalIgnoreCase))
                        {
                            relative = "typings/" + relative.Substring("Scripts/typings/".Length);
                            var tsconfig = Path.Combine(projectDir, "tsconfig.json");
                            if (!File.Exists(tsconfig) ||
                                !File.ReadAllText(tsconfig).Contains(relative, StringComparison.OrdinalIgnoreCase))
                                continue; // old typings only needed for users who didn't fix their tsconfig.json
                        }
                        else if (relative.StartsWith("scripts/", StringComparison.OrdinalIgnoreCase))
                            relative = "Scripts/" + relative.Substring("scripts/".Length);
                        else if (relative.StartsWith("fonts/", StringComparison.OrdinalIgnoreCase))
                            relative = "fonts/" + relative.Substring("fonts/".Length);

                        // all content other than typings go under wwwroot
                        if (!relative.StartsWith("typings/", StringComparison.OrdinalIgnoreCase))
                            relative = "wwwroot/" + relative;

                        restoreFile(file, relative);
                    }
                }

                var typingsRoot = Path.Combine(packageFolder, "typings");
                if (!Directory.Exists(typingsRoot))
                    typingsRoot = Path.Combine(packageFolder, "Typings");

                if (Directory.Exists(typingsRoot))
                {
                    foreach (var file in Directory.GetFiles(typingsRoot, "*.ts", SearchOption.AllDirectories))
                    {
                        var relative = "typings/" + typingsRoot.Substring(contentRoot.Length + 1);
                        restoreFile(file, relative);
                    }
                }

                var nuspecContent = File.ReadAllText(nuspecFile);
                var nuspec = XElement.Parse(nuspecContent);
                var meta = nuspec.Elements().Where(x => x.Name?.LocalName == "metadata").FirstOrDefault();
                if (meta == null)
                    continue;

                var deps = meta.Elements().Where(x => x.Name?.LocalName == "dependencies").FirstOrDefault(); ;
                if (deps == null)
                    continue;

                var fw = dep.Item1;

                var groups = deps.Elements().Where(x => x.Name?.LocalName == "group");
                if (groups.Any())
                {
                    foreach (var group in groups)
                    {
                        var target = group.Attribute("targetFramework").Value;
                        if (string.IsNullOrEmpty(target) ||
                            string.Compare(target, fw, StringComparison.OrdinalIgnoreCase) == 0 ||
                            target.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("netcore", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("net", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var dep2 in group.Elements().Where(x => x.Name?.LocalName == "dependency"))
                            {
                                var id2 = dep2.Attribute("id").Value;
                                if (!skipPackage(id2))
                                    queue.Enqueue((fw, id2, dep2.Attribute("version").Value));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var dep2 in deps.Elements().Where(x => x.Name?.LocalName == "dependency"))
                    {
                        var id2 = dep2.Attribute("id").Value;
                        if (!skipPackage(id2))
                            queue.Enqueue((fw, id2, dep2.Attribute("version").Value));
                    }
                }
            }
        }

        private static IEnumerable<string> EnumerateProjectReferences(string csproj, HashSet<string> visited)
        {
            foreach (var csprojElement in ProjectFileHelper.EnumerateProjectAndDirectoryBuildProps(csproj))
            {
                foreach (var itemGroup in csprojElement.Elements("ItemGroup"))
                {
                    foreach (var projectReference in itemGroup.Elements("ProjectReference"))
                    {
                        var path = projectReference.Attribute("Include")?.Value?.Trim();
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = path.Replace("$(ProjectDir)", Path.GetDirectoryName(Path.GetFullPath(csproj)) +
                                Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

                            path = Path.Combine(Path.GetDirectoryName(csproj), path);
                            if (Directory.Exists(Path.GetDirectoryName(path)))
                                path = Path.GetFullPath(path);
                        }

                        if ((visited == null || !visited.Contains(path)) && File.Exists(path))
                        {
                            visited?.Add(path);
                            yield return path;

                            if (visited != null)
                                foreach (var subReference in EnumerateProjectReferences(path, visited))
                                    yield return subReference;
                        }
                    }
                }
            }
        }

        private static IEnumerable<(string Target, string Id, string Version)> EnumeratePackageReferences(string csproj)
        {
            foreach (var csprojElement in ProjectFileHelper.EnumerateProjectAndDirectoryBuildProps(csproj))
            {
                foreach (var itemGroup in csprojElement.Descendants("ItemGroup"))
                {
                    var condition = itemGroup.Attribute("Condition");
                    var target = "";
                    if (condition != null && !string.IsNullOrEmpty(condition.Value))
                    {
                        const string tf = "'$(TargetFramework)' == '";
                        var idx = condition.Value.IndexOf(tf, StringComparison.Ordinal);
                        if (idx >= 0)
                        {
                            var end = condition.Value.IndexOf("'", idx + tf.Length, StringComparison.Ordinal);
                            if (end >= 0)
                                target = condition.Value.Substring(idx + tf.Length, end - idx - tf.Length);
                        }
                    }

                    foreach (var packageReference in itemGroup.Descendants("PackageReference"))
                    {
                        var ver = packageReference.Attribute("Version")?.Value?.Trim();
                        if (!string.IsNullOrEmpty(ver) &&
                            !Version.TryParse(ver, out _) &&
                            ver.StartsWith("$(", StringComparison.Ordinal) &&
                            ver.EndsWith(")", StringComparison.Ordinal))
                        {
                            var prop = ProjectFileHelper.ExtractPropertyFrom(csproj, xel =>
                                xel.Descendants(ver.Substring(2, ver.Length - 3))?
                                    .LastOrDefault(x => Version.TryParse(x.Value?.TrimToEmpty(), out _))?
                                    .Value?.TrimToNull());
                            if (prop != null)
                                ver = prop;
                        }

                        yield return (target, packageReference.Attribute("Include").Value, ver);
                    }
                }
            }
        }
    }
}