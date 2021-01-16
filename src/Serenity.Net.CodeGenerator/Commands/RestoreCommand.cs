using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using GlobFilter = Serenity.IO.GlobFilter;
using SearchOption = System.IO.SearchOption;

namespace Serenity.CodeGenerator
{
    public class RestoreCommand : BaseFileSystemCommand
    {
        protected IBuildProjectSystem ProjectSystem { get; }

        public RestoreCommand(IFileSystem fileSystem, IBuildProjectSystem projectSystem)
            : base(fileSystem)
        {
            ProjectSystem = projectSystem ?? throw new ArgumentNullException(nameof(projectSystem));
        }

        public ExitCodes Run(string csproj)
        {
            if (csproj == null)
                throw new ArgumentNullException(nameof(csproj));

            if (!File.Exists(csproj))
            {
                Console.Error.WriteLine($"Project file {csproj} is not found!");
                return ExitCodes.ProjectNotFound;
            }

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Func<string, bool> skipPackage = id =>
            {
                if (visited.Contains(id))
                    return true;

                if (CodeGeneration.SkipPackages.ForRestore(id))
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

            try
            {
                foreach (var reference in EnumerateProjectReferences(csproj, new HashSet<string>(StringComparer.OrdinalIgnoreCase)))
                {
                    IBuildProject project;
                    try
                    {
                        project = ProjectSystem.LoadProject(reference);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (var item in project.AllEvaluatedItems
                        .Where(x =>
                            string.Equals(x.ItemType, "TypingsToPackage", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(x.ItemType, "Content", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(x.ItemType, "None", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(x.ItemType, "TypeScriptCompile", StringComparison.OrdinalIgnoreCase))
                        .Where(x => x.EvaluatedInclude?.EndsWith(".d.ts", 
                            StringComparison.OrdinalIgnoreCase) == true))
                    {
                        var sourceFile = Path.Combine(Path.GetDirectoryName(reference),
                            item.EvaluatedInclude);

                        if (!File.Exists(sourceFile))
                            continue;

                        if (!string.Equals(item.ItemType, "TypingsToPackage", StringComparison.OrdinalIgnoreCase) &&
                            item.GetMetadataValue("Pack") != "true")
                            continue;

                        var packagePath = item.GetMetadataValue("PackagePath")?.Trim();
                        if (!string.IsNullOrEmpty(packagePath))
                        {
                            foreach (var path in packagePath.Split(';', StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!PathHelper.ToUrl(path).StartsWith("typings/", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                restoreFile(sourceFile, path);
                                restoredFromProjectReference.Add(path);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            var packagesDir = PackageHelper.DeterminePackagesPath(FileSystem);
            if (packagesDir == null)
            {
                Console.Error.WriteLine("Can't determine NuGet packages directory!");
                return ExitCodes.CantDeterminePackagesDir;
            }

            var queue = new Queue<(string ID, string Version)>();
            foreach (var x in EnumeratePackageReferences(csproj))
            {
                if (!skipPackage(x.Id) && !string.IsNullOrEmpty(x.Version))
                    queue.Enqueue(x);
            };

            while (queue.Count > 0)
            {
                var dep = queue.Dequeue();
                var id = dep.ID;

                var ver = dep.Version.Trim();
                if (ver.EndsWith("-*", StringComparison.Ordinal))
                    ver = ver.Substring(0, ver.Length - 2);
                else if (ver.StartsWith("[", StringComparison.Ordinal) && ver.EndsWith("]", StringComparison.Ordinal))
                {
                    ver = ver[1..^1].Trim();
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
                            relative = "Content/" + relative["content/".Length..];
                        else if (relative.StartsWith("scripts/typings/", StringComparison.OrdinalIgnoreCase))
                        {
                            relative = "typings/" + relative["Scripts/typings/".Length..];
                            var tsconfig = Path.Combine(projectDir, "tsconfig.json");
                            if (!File.Exists(tsconfig) ||
                                !File.ReadAllText(tsconfig).Contains(relative, StringComparison.OrdinalIgnoreCase))
                                continue; // old typings only needed for users who didn't fix their tsconfig.json
                        }
                        else if (relative.StartsWith("scripts/", StringComparison.OrdinalIgnoreCase))
                            relative = "Scripts/" + relative["scripts/".Length..];
                        else if (relative.StartsWith("fonts/", StringComparison.OrdinalIgnoreCase))
                            relative = "fonts/" + relative["fonts/".Length..];

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
                        var relative = "typings/" + file[(typingsRoot.Length + 1)..];
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

                var fw = dep.ID;

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
                                    queue.Enqueue((id2, dep2.Attribute("version").Value));
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
                            queue.Enqueue((id2, dep2.Attribute("version").Value));
                    }
                }
            }

            return ExitCodes.Success;
        }

        private IEnumerable<string> EnumerateProjectReferences(string csproj, HashSet<string> visited, int depth = 0)
        {
            var allReferences = new List<string>();
            try
            {
                csproj = Path.GetFullPath(csproj);
                var project = ProjectSystem.LoadProject(csproj);
                visited?.Add(csproj);

                foreach (var item in project.AllEvaluatedItems)
                {
                    if (string.Equals(item.ItemType, "ProjectReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(item.EvaluatedInclude))
                        {
                            var path = Path.Combine(Path.GetDirectoryName(csproj), item.EvaluatedInclude);
                            if (File.Exists(path) && visited?.Contains(path) != true)
                            {
                                path = Path.GetFullPath(path);
                                allReferences.Add(path);

                                if (visited != null && depth < 5)
                                {
                                    foreach (var subref in EnumerateProjectReferences(path, visited, depth + 1))
                                        allReferences.Add(subref);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return allReferences.Distinct().ToList();
        }

        private IEnumerable<(string Id, string Version)> EnumeratePackageReferences(string csproj)
        {
            IBuildProject project;
            try
            {
                csproj = Path.GetFullPath(csproj);
                project = ProjectSystem.LoadProject(csproj);
            }
            catch
            {
                yield break;
            }

            foreach (var item in project.AllEvaluatedItems)
            {
                if (string.Equals(item.ItemType, "PackageReference", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(item.EvaluatedInclude))
                    yield return (item.EvaluatedInclude, item.GetMetadataValue("Version"));
            }
        }
    }
}