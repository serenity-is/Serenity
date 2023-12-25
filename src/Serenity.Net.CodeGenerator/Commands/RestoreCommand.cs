using System.Xml.Linq;
using GlobFilter = Serenity.IO.GlobFilter;

namespace Serenity.CodeGenerator;

public class RestoreCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public IEnumerable<string> ProjectReferences { get; set; }
    public IBuildProjectSystem BuildSystem { get; set; }

    public bool Verbose { get; set; }

    public override ExitCodes Run()
    {
        ArgumentNullException.ThrowIfNull(BuildSystem);

        if (!FileSystem.FileExists(ProjectFile))
        {
            if (Verbose)
                Console.Error($"Project file {ProjectFile} is not found!");
            return ExitCodes.ProjectNotFound;
        }

        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        if (config?.Restore?.Exclude?.Any(x => x == "**/*") == true)
            return ExitCodes.Success;

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        bool skipPackage(string id)
        {
            if (visited.Contains(id))
                return true;

            if (CodeGeneration.SkipPackages.ForRestore(id))
                return true;

            visited.Add(id);

            return false;
        }

        GlobFilter include = null;
        if (config.Restore?.Include.IsEmptyOrNull() == false)
            include = new GlobFilter(config.Restore.Include);

        GlobFilter exclude = null;
        if (config.Restore?.Exclude.IsEmptyOrNull() == false)
            exclude = new GlobFilter(config.Restore.Exclude);

        var targetRoot = FileSystem.GetDirectoryName(ProjectFile);
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

            var target = FileSystem.Combine(targetRoot, relative);
            if (FileSystem.FileExists(target))
            {
                if (!FileSystem.ReadAllBytes(target)
                    .SequenceEqual(FileSystem.ReadAllBytes(file)))
                {
                    Console.WriteLine("Restoring: " + relative, ConsoleColor.Green);
                    FileSystem.Copy(file, target, overwrite: true);
                }
            }
            else
            {
                if (!FileSystem.DirectoryExists(target))
                    FileSystem.CreateDirectory(FileSystem.GetDirectoryName(target));

                Console.WriteLine("Restoring: " + relative, ConsoleColor.Green);
                FileSystem.Copy(file, target, overwrite: false);
            }
        }

        if (config?.Restore?.Typings != false)
            try
            {
                var projectRefs = ProjectReferences?.Where(x =>
                    !IgnoreProjectRefs.Contains(FileSystem.GetFileNameWithoutExtension(x))) ??
                        EnumerateProjectReferences(ProjectFile, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

                foreach (var reference in projectRefs)
                {
                    if (Verbose)
                        Console.WriteLine("Project Reference: " + reference);

                    IBuildProject project;
                    try
                    {
                        project = BuildSystem.LoadProject(reference);
                    }
                    catch
                    {
                        if (Verbose)
                            Console.WriteLine("Could not Load Project Reference!: " + reference);

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
                        var sourceFile = PathHelper.ToPath(FileSystem.Combine(FileSystem.GetDirectoryName(reference),
                            item.EvaluatedInclude));

                        if (Verbose)
                            Console.WriteLine("Checking source file: " + sourceFile);

                        if (!FileSystem.FileExists(sourceFile))
                        {
                            if (Verbose)
                                Console.WriteLine("Source file does NOT exist: " + sourceFile);
                            continue;
                        }

                        if (Verbose)
                            Console.WriteLine("Source file exists: " + sourceFile);

                        if (!string.Equals(item.ItemType, "TypingsToPackage", StringComparison.OrdinalIgnoreCase) &&
                            item.GetMetadataValue("Pack") != "true")
                            continue;

                        var packagePath = item.GetMetadataValue("PackagePath")?.Trim();
                        if (!string.IsNullOrEmpty(packagePath))
                        {
                            foreach (var path in packagePath.Split(';', StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (Verbose)
                                    Console.WriteLine("Checking project package path " + path);

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
                Console.Error(ex.Message);
            }

        var packagesDir = PackageHelper.DeterminePackagesPath(FileSystem, Console);
        if (packagesDir == null)
        {
            Console.Error("Can't determine NuGet packages directory!");
            return ExitCodes.CantDeterminePackagesDir;
        }

        var queue = new Queue<(string ID, string Version)>();
        foreach (var x in EnumeratePackageReferences(ProjectFile))
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
                ver = ver[0..^2];
            else if (ver.StartsWith('[') && ver.EndsWith(']'))
            {
                ver = ver[1..^1].Trim();
            }

            var packageFolder = FileSystem.Combine(FileSystem.Combine(packagesDir, id), ver);
            if (!FileSystem.DirectoryExists(packageFolder))
            {
                packageFolder = FileSystem.Combine(FileSystem.Combine(packagesDir, id.ToLowerInvariant()), ver);
                if (!FileSystem.DirectoryExists(packageFolder))
                {
                    var myPackagesDir = FileSystem.Combine(packagesDir, "..", "my-packages");
                    packageFolder = FileSystem.Combine(myPackagesDir, id, ver);
                    if (!FileSystem.DirectoryExists(packageFolder))
                        packageFolder = FileSystem.Combine(myPackagesDir, id.ToLowerInvariant(), ver);
                }
            }

            var nuspecFile = FileSystem.Combine(packageFolder, id + ".nuspec");
            if (!FileSystem.FileExists(nuspecFile))
            {
                nuspecFile = FileSystem.Combine(packageFolder, id.ToLowerInvariant() + ".nuspec");
                if (!FileSystem.FileExists(nuspecFile))
                {
                    if (Verbose)
                        Console.WriteLine("Can't find nuspec file: " + nuspecFile);
                    continue;
                }
            }

            Console.WriteLine("Processing: " + id, ConsoleColor.Cyan);

            var contentRoot = FileSystem.Combine(packageFolder, "content");
            if (!FileSystem.DirectoryExists(contentRoot))
                contentRoot = FileSystem.Combine(packageFolder, "Content");

            if (FileSystem.DirectoryExists(contentRoot))
            {
                foreach (var file in FileSystem.GetFiles(contentRoot, "*.*", recursive: true))
                {
                    var extension = FileSystem.GetExtension(file);
                    if (string.Compare(extension, ".transform", StringComparison.OrdinalIgnoreCase) == 0)
                        continue;

                    var relative = PathHelper.ToUrl(file[(contentRoot.Length + 1)..]);

                    // normalize paths as Content, Scripts, fonts and typings (these are exact cases expected)
                    if (relative.StartsWith("content/", StringComparison.OrdinalIgnoreCase))
                        relative = "Content/" + relative["content/".Length..];
                    else if (relative.StartsWith("scripts/typings/", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config?.Restore.Typings != false)
                        {
                            relative = "typings/" + relative["Scripts/typings/".Length..];
                            var tsconfig = FileSystem.Combine(projectDir, "tsconfig.json");
                            if (!FileSystem.FileExists(tsconfig) ||
                                !FileSystem.ReadAllText(tsconfig).Contains(relative, StringComparison.OrdinalIgnoreCase))
                                continue; // old typings only needed for users who didn't fix their tsconfig.json
                        }
                    }
                    else if (relative.StartsWith("scripts/", StringComparison.OrdinalIgnoreCase))
                        relative = "Scripts/" + relative["scripts/".Length..];
                    else if (relative.StartsWith("fonts/", StringComparison.OrdinalIgnoreCase))
                        relative = "fonts/" + relative["fonts/".Length..];

                    // all content other than typings go under wwwroot
                    if (!relative.StartsWith("typings/", StringComparison.OrdinalIgnoreCase))
                        relative = "wwwroot/" + relative;

                    if (Verbose)
                        Console.WriteLine("Found a file to restore: " + relative);
                    restoreFile(file, relative);
                }
            }
            else if (Verbose)
            {
                Console.WriteLine("Can't find package content directory: " + nuspecFile);
            }

            var typingsRoot = FileSystem.Combine(packageFolder, "typings");
            if (!FileSystem.DirectoryExists(typingsRoot))
                typingsRoot = FileSystem.Combine(packageFolder, "Typings");

            if (FileSystem.DirectoryExists(typingsRoot))
            {
                foreach (var file in FileSystem.GetFiles(typingsRoot, "*.ts", recursive: true))
                {
                    if (Verbose)
                    {
                        Console.WriteLine(typingsRoot);
                        Console.WriteLine(file);
                    }
                    var relative = "typings/" + file[(typingsRoot.Length + 1)..];
                    restoreFile(file, relative);
                }
            }

            var nuspecContent = FileSystem.ReadAllText(nuspecFile);
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

    private static readonly HashSet<string> IgnoreProjectRefs = new(StringComparer.OrdinalIgnoreCase)
    {
        "Serenity.Net.Core",
        "Serenity.Net.Data",
        "Serenity.Net.Entity",
        "Serenity.Net.Services",
        "Serenity.Net.Web"
    };

    private List<string> EnumerateProjectReferences(string csproj, HashSet<string> visited, int depth = 0)
    {
        var allReferences = new List<string>();
        try
        {
            csproj = FileSystem.GetFullPath(csproj);
            var project = BuildSystem.LoadProject(csproj);
            visited?.Add(csproj);

            foreach (var item in project.AllEvaluatedItems)
            {
                if (string.Equals(item.ItemType, "ProjectReference",
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(item.EvaluatedInclude))
                        continue;

                    var path = FileSystem.Combine(FileSystem.GetDirectoryName(csproj), item.EvaluatedInclude);
                    if (!FileSystem.FileExists(path) || visited?.Contains(path) == true)
                        continue;

                    if (IgnoreProjectRefs.Contains(FileSystem.GetFileNameWithoutExtension(path)))
                        continue;

                    path = FileSystem.GetFullPath(path);
                    allReferences.Add(path);
                    if (visited?.Contains(path) == true)
                        continue;

                    if (visited != null && depth < 5)
                    {
                        foreach (var subref in EnumerateProjectReferences(path, visited, depth + 1))
                            allReferences.Add(subref);
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
            csproj = FileSystem.GetFullPath(csproj);
            project = BuildSystem.LoadProject(csproj);
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