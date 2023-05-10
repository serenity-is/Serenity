namespace Serenity.CodeGenerator;

public class TSModuleResolver
{
    private readonly IGeneratorFileSystem fileSystem;
    private readonly string tsBasePath;
    private readonly Dictionary<string, string[]> paths;

    private readonly string[] extensions = new string[]
    {
            ".ts",
            ".tsx",
            ".d.ts"
    };

    private static readonly Regex removeMultiSlash = new(@"\/+");

    public TSModuleResolver(IGeneratorFileSystem fileSystem, string tsConfigDir, TSConfig tsConfig)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        tsBasePath = tsConfig is null || tsConfig.CompilerOptions?.BaseUrl is null ||
            tsConfig.CompilerOptions?.BaseUrl == "." || tsConfig.CompilerOptions?.BaseUrl == "./" ? tsConfigDir :
            fileSystem.Combine(tsConfigDir, PathHelper.ToPath(tsConfig.CompilerOptions?.BaseUrl));

        tsBasePath = PathHelper.ToPath(tsBasePath);

        paths = tsConfig?.CompilerOptions?.Paths ?? new Dictionary<string, string[]>();
    }

    private class PackageJson
    {
        public string Name { get; set; }
        public Dictionary<string, Dictionary<string, string[]>> TypesVersions { get; set; }
        public string Types { get; set; }
        public string Typings { get; set; }
    }

    static string RemoveTrailing(string path)
    {
        while (path != null && path.Length > 1 &&
            path.EndsWith("\\", StringComparison.Ordinal) ||
            path.EndsWith("/", StringComparison.Ordinal))
                path = path[..^1];
        return path;
    }

    string TryGetNodePackageName(string path)
    {
        path = RemoveTrailing(PathHelper.ToUrl(path));
        var lastNodeIdx = path.LastIndexOf("/node_modules/", StringComparison.Ordinal);
        if (lastNodeIdx <= 0)
            return null;

        var remaining = RemoveTrailing(path[(lastNodeIdx + "/node_modules/".Length) ..]);
        while (remaining.StartsWith("/", StringComparison.Ordinal))
            remaining = remaining[1..];
        if (remaining.Length > 0)
        {
            var parts = remaining.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (remaining.StartsWith("@"))
                return string.Join("/", parts.Take(2));

            return parts.FirstOrDefault();
        }

        return null;
    }

    public string Resolve(string fileNameOrModule, string referencedFrom,
        out string moduleName)
    {
        moduleName = null;

        if (string.IsNullOrEmpty(fileNameOrModule))
            return null;

        string resolvedPath = null;

        if (string.IsNullOrEmpty(referencedFrom))
        {
            resolvedPath = fileNameOrModule;
            if (!fileSystem.FileExists(resolvedPath))
                return null;
        }
        else if (fileNameOrModule.StartsWith(".", StringComparison.Ordinal) ||
            fileNameOrModule.StartsWith("/", StringComparison.Ordinal))
        {
            string relative = removeMultiSlash.Replace(PathHelper.ToUrl(fileNameOrModule), "/");

            var searchBase = fileNameOrModule.StartsWith("/", StringComparison.Ordinal) ?
                tsBasePath : fileSystem.GetDirectoryName(RemoveTrailing(referencedFrom));

            if (fileNameOrModule.StartsWith("./", StringComparison.Ordinal))
                relative = relative[2..];
            else if (!fileNameOrModule.StartsWith("../", StringComparison.Ordinal))
                relative = relative[1..];

            var withoutSlash = relative.EndsWith("/", StringComparison.Ordinal) ? 
                relative[..^1] : relative;

            if (!string.IsNullOrEmpty(withoutSlash))
                searchBase = fileSystem.Combine(searchBase, withoutSlash);

            if (fileNameOrModule == "." || 
                fileNameOrModule.EndsWith("/", StringComparison.Ordinal))
                resolvedPath = extensions
                    .Select(ext => fileSystem.Combine(searchBase, "index" + ext))
                    .FirstOrDefault(fileSystem.FileExists);
            else
            {
                resolvedPath = extensions
                        .Select(ext => searchBase + ext)
                        .FirstOrDefault(fileSystem.FileExists) ??
                    extensions
                        .Select(ext => fileSystem.Combine(searchBase + "/index" + ext))
                        .FirstOrDefault(fileSystem.FileExists);
            }
        }
        else
        {
            void tryPackageJson(string path, ref string moduleName)
            {
                var packageJson = TSConfigHelper.TryParseJsonFile<PackageJson>(fileSystem, 
                    fileSystem.Combine(path, "package.json"));

                if (packageJson is not null)
                {
                    var types = packageJson.Types ?? packageJson.Typings;
                    if (!string.IsNullOrEmpty(types) &&
                        fileSystem.FileExists(fileSystem.Combine(path, types)))
                    {
                        resolvedPath = fileSystem.Combine(path, types);
                        moduleName = packageJson.Name ?? TryGetNodePackageName(path) ?? fileSystem.GetFileName(path);
                    }
                    return;
                }

                var parentDir = fileSystem.GetDirectoryName(RemoveTrailing(path));

                packageJson = TSConfigHelper.TryParseJsonFile<PackageJson>(fileSystem,
                    fileSystem.Combine(parentDir, "package.json"));

                if (packageJson is not null)
                {
                    if (packageJson.TypesVersions is not null &&
                        packageJson.TypesVersions.TryGetValue("*", out var tv) &&
                        tv.TryGetValue(fileSystem.GetFileName(path), out var typesArr) &&
                        typesArr is not null)
                    {
                        resolvedPath = typesArr
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => fileSystem.Combine(parentDir, x))
                            .FirstOrDefault(x => fileSystem.FileExists(x));

                        if (resolvedPath is not null)
                        {
                            moduleName = (packageJson.Name ?? fileSystem.GetFileName(parentDir)) +
                                "/" + fileSystem.GetFileName(path);
                            return;
                        }
                    }

                    var types = packageJson.Types ?? packageJson.Typings;
                    if (!string.IsNullOrEmpty(types) &&
                        fileSystem.FileExists(fileSystem.Combine(parentDir, types)))
                    {
                        resolvedPath = fileSystem.Combine(parentDir, types);
                        moduleName = packageJson.Name ?? TryGetNodePackageName(parentDir) ?? fileSystem.GetFileName(parentDir);
                        return;
                    }
                }
            }

            void tryPath(string testBase, ref string moduleName)
            {
                if (testBase[^1] != '\\' && testBase[^1] != '/')
                    resolvedPath = extensions
                        .Select(ext => testBase + ext)
                        .FirstOrDefault(fileSystem.FileExists);

                if (resolvedPath is null)
                    tryPackageJson(testBase, ref moduleName);

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
                        (pattern.EndsWith("*") &&
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

                                if (toCombine.StartsWith("/", StringComparison.Ordinal))
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
                        tryPackageJson(fileSystem.Combine(nodeModules, fileNameOrModule), ref moduleName);
                    parentDir = fileSystem.GetDirectoryName(RemoveTrailing(parentDir));
                }
            }
        }

        if (resolvedPath is null)
            return null;

        resolvedPath = PathHelper.ToPath(fileSystem.GetFullPath(resolvedPath));

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

        return resolvedPath;
    }
}