#if ISSOURCEGENERATOR
using Serenity.TypeScript;
using System.Collections.Concurrent;
#endif

namespace Serenity.CodeGenerator;

public class ResolveResult
{
    public string FullPath { get; set; }
    public string ModuleName { get; set; }
    public string ActualPath { get; set; }
}

public partial class TSModuleResolver
{
    private readonly IFileSystem fileSystem;
    private readonly string tsBasePath;
    private readonly Dictionary<string, string[]> paths;

    private readonly string[] extensions =
    [
        ".ts",
        ".tsx",
        ".d.ts"
    ];

#if ISSOURCEGENERATOR
    private static readonly Regex removeMultiSlash = new(@"\/+", RegexOptions.Compiled);
#else
    [GeneratedRegex(@"\/+", RegexOptions.Compiled)]
    private static partial Regex removeMultiSlashRegexGen();

    private static readonly Regex removeMultiSlash = removeMultiSlashRegexGen();
#endif
    private static readonly char[] slashSeparator = ['/'];

    public TSModuleResolver(IFileSystem fileSystem, string tsConfigDir, TSConfig tsConfig)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        tsBasePath = tsConfig is null || tsConfig.CompilerOptions?.BaseUrl is null ||
            tsConfig.CompilerOptions?.BaseUrl == "." || tsConfig.CompilerOptions?.BaseUrl == "./" ? tsConfigDir :
            fileSystem.Combine(tsConfigDir, PathHelper.ToPath(tsConfig.CompilerOptions?.BaseUrl));

        tsBasePath = PathHelper.ToPath(tsBasePath);

        paths = tsConfig?.CompilerOptions?.Paths ?? [];
    }

    private class PackageJson
    {
        public string Name { get; set; }
        public Dictionary<string, Dictionary<string, string[]>> TypesVersions { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
        public Dictionary<string, string> DevDependencies { get; set; }
        public Dictionary<string, string> PeerDependencies { get; set; }
        public string Types { get; set; }
        public string Typings { get; set; }
    }

    static string RemoveTrailing(string path)
    {
        while (path != null && path.Length > 1 &&
            path.EndsWith('\\') ||
            path.EndsWith('/'))
            path = path[..^1];
        return path;
    }

    static string TryGetNodePackageName(string path)
    {
        path = RemoveTrailing(PathHelper.ToUrl(path));
        var lastNodeIdx = path.LastIndexOf("/node_modules/", StringComparison.Ordinal);
        if (lastNodeIdx <= 0)
            return null;

        var remaining = RemoveTrailing(path[(lastNodeIdx + "/node_modules/".Length)..]);
        while (remaining.StartsWith('/'))
            remaining = remaining[1..];
        if (remaining.Length > 0)
        {
            if (remaining.StartsWith(".dotnet/"))
            {
                remaining = remaining[(".dotnet/".Length)..];
                if (remaining.Length == 0)
                    return null;
            }

            var parts = remaining.Split(slashSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (remaining.StartsWith('@'))
                return string.Join("/", parts.Take(2));

            return parts.FirstOrDefault();
        }

        return null;
    }

    private readonly ConcurrentDictionary<string, Lazy<PackageJson>> packageJson = new();

    private PackageJson TryParsePackageJson(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        var cacheKey = TypeScript.Utilities.NormalizePath(path);

        return packageJson.GetOrAdd(cacheKey, cacheKey => new(() => TSConfigHelper.TryParseJsonFile<PackageJson>(fileSystem, path))).Value;
    }

    private string ResolveRelative(string relativePath, string referencedFrom)
    {
        string relative = removeMultiSlash.Replace(PathHelper.ToUrl(relativePath), "/");

        var searchBase = relativePath.StartsWith('/') ?
            tsBasePath : fileSystem.GetDirectoryName(RemoveTrailing(referencedFrom));

        if (relativePath.StartsWith("./", StringComparison.Ordinal))
            relative = relative[2..];
        else if (!relativePath.StartsWith("../", StringComparison.Ordinal))
            relative = relative[1..];

        var withoutSlash = relative.EndsWith('/') ?
            relative[..^1] : relative;

        if (!string.IsNullOrEmpty(withoutSlash))
            searchBase = fileSystem.Combine(searchBase, withoutSlash);

        if (relativePath == "." ||
            relativePath.EndsWith('/'))
            return extensions
                .Select(ext => fileSystem.Combine(searchBase, "index" + ext))
                .FirstOrDefault(fileSystem.FileExists);

        return extensions
                .Select(ext => searchBase + ext)
                .FirstOrDefault(fileSystem.FileExists) ??
            extensions
                .Select(ext => fileSystem.Combine(searchBase + "/index" + ext))
                .FirstOrDefault(fileSystem.FileExists);
    }

    public ResolveResult Resolve(string fileNameOrModule, string referencedFrom)
    {
        if (string.IsNullOrEmpty(fileNameOrModule))
            return null;

        string resolvedPath = null;
        string moduleName = null;
        string actualPath = null;

        ResolveResult createResult()
        {
            if (resolvedPath is null)
                return null;

            resolvedPath = PathHelper.ToPath(fileSystem.GetFullPath(resolvedPath));
            if (actualPath is not null)
                actualPath = PathHelper.ToPath(fileSystem.GetFullPath(actualPath));

            moduleName ??= TryGetNodePackageName(resolvedPath);
            if (moduleName is null)
            {
                var fromPath = tsBasePath;
                if (!fromPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    fromPath += System.IO.Path.DirectorySeparatorChar;
                moduleName = '/' + PathHelper.ToUrl(fileSystem.GetRelativePath(fromPath, resolvedPath));

                foreach (var ext in extensions.Reverse())
                {
                    if (moduleName.EndsWith(ext))
                    {
                        moduleName = moduleName[..^ext.Length];
                        break;
                    }
                }

                if (moduleName.EndsWith("/index")) // leave slash at end
                    moduleName = moduleName[..^"index".Length];
            }

            return new ResolveResult
            {
                FullPath = resolvedPath,
                ModuleName = moduleName,
                ActualPath = actualPath
            };
        }

        if (string.IsNullOrEmpty(referencedFrom))
        {
            resolvedPath = fileNameOrModule;
            if (!fileSystem.FileExists(resolvedPath))
                return null;
            return createResult();
        }

        if (fileNameOrModule.StartsWith('.') ||
            fileNameOrModule.StartsWith('/'))
        {
            resolvedPath = ResolveRelative(fileNameOrModule, referencedFrom);
            return createResult();
        }

        void tryPath(string testBase, ref string moduleName)
        {
            if (testBase[^1] != '\\' && testBase[^1] != '/')
                resolvedPath = extensions
                    .Select(ext => testBase + ext)
                    .FirstOrDefault(fileSystem.FileExists);

            resolvedPath ??= TryResolveFromPackageFolder(testBase, ref moduleName);
            resolvedPath ??= extensions
                .Select(ext => fileSystem.Combine(testBase, "index" + ext))
                .FirstOrDefault(fileSystem.FileExists);
        }

        if (paths != null)
        {
            foreach (var pattern in paths.Keys)
            {
                if (pattern == fileNameOrModule ||
                    pattern == "*" ||
                    (pattern.EndsWith('*') &&
                        fileNameOrModule.StartsWith(pattern[..^1])))
                {
                    if (paths[pattern] is string[] mappings)
                    {
                        var replaceWith = pattern == "*" ? fileNameOrModule :
                            fileNameOrModule[(pattern.Length - 1)..];

                        foreach (var mapping in mappings)
                        {
                            if (string.IsNullOrEmpty(mapping))
                                continue;

                            var toCombine = removeMultiSlash.Replace(PathHelper.ToUrl(mapping), "/");
                            if (toCombine.StartsWith("./"))
                                toCombine = toCombine[2..];
                            else if (toCombine.StartsWith('/'))
                                continue; // not supported?

                            toCombine = toCombine.Replace("*", replaceWith);

                            var testBase = tsBasePath;
                            if (!string.IsNullOrEmpty(toCombine))
                                testBase = fileSystem.Combine(testBase, PathHelper.ToPath(toCombine));

                            tryPath(testBase, ref moduleName);

                            if (resolvedPath is not null)
                                break;
                        }
                    }
                }

                if (resolvedPath is not null)
                    break;
            }
        }

        if (resolvedPath is null &&
            !string.IsNullOrEmpty(referencedFrom))
        {
            var parentDir = fileSystem.GetDirectoryName(RemoveTrailing(referencedFrom));
            while (resolvedPath is null &&
                !string.IsNullOrEmpty(parentDir))
            {
                var nodeModules = fileSystem.Combine(parentDir, "node_modules");
                if (fileSystem.DirectoryExists(nodeModules))
                {
                    resolvedPath = TryResolveFromPackageFolder(fileSystem.Combine(
                        nodeModules, fileNameOrModule), ref moduleName);

                    if (resolvedPath == null)
                    {
                        var searchBase = fileSystem.Combine(nodeModules, PathHelper.ToPath(fileNameOrModule));
                        var index = fileSystem.Combine(searchBase, "index.d.ts");
                        if (fileSystem.FileExists(index))
                        {
                            moduleName = fileNameOrModule;
                            resolvedPath = index;
                            break;
                        }
                    }
                    else
                        break;
                }
                parentDir = fileSystem.GetDirectoryName(RemoveTrailing(parentDir));
            }
        }

        if (resolvedPath is null)
        {
            actualPath = TryResolveFromDependencies(fileNameOrModule);
            if (actualPath is not null)
            {
                moduleName = fileNameOrModule;
                resolvedPath = fileSystem.Combine(rootPackageJsonFolder, "node_modules", moduleName);
            }
        }

        return createResult();
    }

    private Dictionary<string, string> allDependencies;
    private string rootPackageJsonFolder;

    private Dictionary<string, string> GetAllDependencies()
    {
        if (allDependencies is not null)
            return allDependencies;

        PackageJson packageJson;
        var path = tsBasePath;
        do
        {
            packageJson = TryParsePackageJson(fileSystem.Combine(path, "package.json"));
        }
        while (packageJson is null &&
            !string.IsNullOrEmpty(path) &&
            !string.IsNullOrEmpty(path = fileSystem.GetDirectoryName(path)));

        if (packageJson is not null)
            rootPackageJsonFolder = path;

        Dictionary<string, string> allDeps = [];
        void addDeps(Dictionary<string, string> source)
        {
            if (source is not null)
                foreach (var pair in source)
                    if (!allDeps.ContainsKey(pair.Key))
                        allDeps.Add(pair.Key, pair.Value);
        }
        addDeps(packageJson?.Dependencies);
        addDeps(packageJson?.DevDependencies);
        addDeps(packageJson?.PeerDependencies);

        return (allDependencies = allDeps);
    }

    string TryResolveFromPackageFolder(string path, ref string moduleName)
    {
        var packageJson = TryParsePackageJson(fileSystem.Combine(path, "package.json"));

        string withPackageJson(ref string moduleName)
        {
            var types = packageJson.Types ?? packageJson.Typings;
            if (!string.IsNullOrEmpty(types) &&
                fileSystem.FileExists(fileSystem.Combine(path, types)))
            {
                moduleName = packageJson.Name ?? TryGetNodePackageName(path) ?? fileSystem.GetFileName(path);
                return fileSystem.Combine(path, types);
            }
            return null;
        }

        if (packageJson is not null)
            return withPackageJson(ref moduleName);

        var parentDir = fileSystem.GetDirectoryName(RemoveTrailing(path));
        packageJson = TryParsePackageJson(fileSystem.Combine(parentDir, "package.json"));

        if (packageJson is null || fileSystem.GetFileName(parentDir) == "node_modules")
            return null;

        if (packageJson.TypesVersions is not null &&
            packageJson.TypesVersions.TryGetValue("*", out var tv) &&
            tv.TryGetValue(fileSystem.GetFileName(path), out var typesArr) &&
            typesArr is not null)
        {
            var resolvedPath = typesArr
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => fileSystem.Combine(parentDir, x))
                .FirstOrDefault(x => fileSystem.FileExists(x));

            if (resolvedPath is not null)
            {
                moduleName = (packageJson.Name ?? fileSystem.GetFileName(parentDir)) +
                    "/" + fileSystem.GetFileName(path);
                return resolvedPath;
            }
        }

        return withPackageJson(ref moduleName);
    }

    private string TryResolveFromDependencies(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return null;

        var allDependencies = GetAllDependencies();
        if (!allDependencies.TryGetValue(moduleName, out var value))
            return null;

        var nugetPackages = fileSystem.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        var packageRoot = fileSystem.Combine(nugetPackages,
            GetPossibleNuGetPackageId(moduleName));

        if (Version.TryParse(value, out _))
        {
            var packageIndex = fileSystem.Combine(packageRoot, value, "dist", "index.d.ts");
            if (fileSystem.FileExists(packageIndex))
                return packageIndex;
        }
        else
        {
            var relativePath = value.StartsWith("file:", StringComparison.Ordinal) && value.Length > 5 ?
                value[5..] : ((value.StartsWith("./", StringComparison.Ordinal) ||
                    value.StartsWith("../", StringComparison.Ordinal)) ? value : null);

            if (relativePath != null)
            {
                var packageIndex = fileSystem.Combine(rootPackageJsonFolder, relativePath, "dist", "index.d.ts");
                if (fileSystem.FileExists(packageIndex))
                    return packageIndex;
            }
        }

        if (!fileSystem.DirectoryExists(packageRoot))
            return null;

        foreach (var directory in fileSystem.GetDirectories(packageRoot)
            .OrderByDescending(x => Version.TryParse(x, out var version) ? version : new Version(0, 0, 0)))
        {
            var packageIndex = fileSystem.Combine(packageRoot, value, "dist", "index.d.ts");
            if (fileSystem.FileExists(packageIndex))
                return packageIndex;
        }

        return null;
    }

    private readonly char[] packageIdSplitChars = ['-', '.', '/'];

    private string GetPossibleNuGetPackageId(string moduleName)
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