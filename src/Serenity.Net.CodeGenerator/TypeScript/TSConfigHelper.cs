using System.Threading;

namespace Serenity.CodeGenerator;

public static class TSConfigHelper
{
    public static IEnumerable<string> ListFiles(
        IFileSystem fileSystem, string configPath, out TSConfig tsConfig,
        CancellationToken cancellationToken = default)
    {
        tsConfig = Read(fileSystem, configPath);
        if (tsConfig is null)
            return null;

        return ListFiles(tsConfig, fileSystem, fileSystem.GetDirectoryName(configPath), cancellationToken);
    }

    public static IEnumerable<string> ListFiles(TSConfig config,
        IFileSystem fileSystem, string rootDir,
        CancellationToken cancellationToken = default)
    {
        ArgumentExceptionHelper.ThrowIfNull(config);

        if (config.Files != null)
        {
            return config.Files.Where(x => fileSystem.FileExists(
                    fileSystem.Combine(rootDir, PathHelper.ToPath(x))))
                .Select(x => fileSystem.GetFullPath(
                    fileSystem.Combine(rootDir, PathHelper.ToPath(x))));
        }

        var include = config.Include;
        if (include == null)
        {
            if (config.Exclude == null || config.Exclude.Length == 0)
                return null;

            include = ["**/*"];
        }

        var typeRoots = config.CompilerOptions?.TypeRoots?.IsEmptyOrNull() != false ?
            ["./node_modules/@types"] : config.CompilerOptions.TypeRoots;

        var types = new HashSet<string>(config.CompilerOptions?.Types ??
            [], StringComparer.OrdinalIgnoreCase);

        IEnumerable<string> files = typeRoots.Select(typeRoot =>
        {
            var s = PathHelper.ToUrl(typeRoot);
            if (s.StartsWith("./", StringComparison.Ordinal))
                s = s[2..];
            return fileSystem.Combine(rootDir, PathHelper.ToPath(s));
        })
        .Where(fileSystem.DirectoryExists)
        .Select(fileSystem.GetFullPath)
        .SelectMany(typeRoot =>
            fileSystem.GetDirectories(typeRoot)
                .Where(typing => (config.CompilerOptions?.Types == null) || types.Contains(fileSystem.GetDirectoryName(typing)))
                .Where(typing => fileSystem.GetFileName(typing).Contains("serenity", StringComparison.OrdinalIgnoreCase) ||
                    !PathHelper.ToUrl(typing).Contains("/node_modules/", StringComparison.OrdinalIgnoreCase))
                .Select(typing => fileSystem.Combine(typing, "index.d.ts"))
                .Where(typing => fileSystem.FileExists(typing)))
        .ToList();

        cancellationToken.ThrowIfCancellationRequested();

        var includePatterns = config.Include
            .Select(PathHelper.ToUrl)
            .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
            .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                (x.StartsWith('/') ? x[1..] : x))
            .Select(x => (!x.StartsWith("**/", StringComparison.Ordinal) && !x.StartsWith('/')) ? ("/" + x) : x)
            .ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var excludePatterns = (config.Exclude ?? [])
            .Select(PathHelper.ToUrl)
            .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
            .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                (x.StartsWith('/') ? x[1..] : x))
            .ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        files = files.Concat(config.Include.Select(PathHelper.ToUrl)
            .Where(x => x.StartsWith("../", StringComparison.Ordinal) &&
            !x.Contains('*', StringComparison.Ordinal))
            .Select(x => fileSystem.Combine(rootDir, x))
            .Select(PathHelper.ToPath)
            .Where(fileSystem.FileExists));

        cancellationToken.ThrowIfCancellationRequested();

        var includeGlob = new IO.GlobFilter(includePatterns);
        var excludeGlob = new IO.GlobFilter(excludePatterns);

        IEnumerable<string> allTsFiles;

        if (includePatterns.Length > 0 &&
            includePatterns.All(x =>
                x.StartsWith('/') &&
                !x.StartsWith(@"/*", StringComparison.Ordinal) &&
                x[1..].Split('/')[0].IndexOf('*') < 0))
        {
            // may optimize by traversing directories at root manually, e.g. skip node_modules etc.
            var scanDirs = includePatterns.Select(x =>
                x[1..].Split('/')[0].Trim())
                .Where(y => !string.IsNullOrEmpty(y))
                .ToArray();

            allTsFiles = fileSystem.GetFiles(rootDir, "*.ts");
            foreach (var directory in fileSystem.GetDirectories(rootDir))
            {
                var directoryName = fileSystem.GetFileName(directory);
                if (!scanDirs.Any(x => string.Equals(x, directoryName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                allTsFiles = allTsFiles.Concat(fileSystem.GetFiles(directory, "*.ts", recursive: true));
            }
            allTsFiles = allTsFiles.ToArray();
        }
        else
        {
            allTsFiles = fileSystem.GetFiles(rootDir, "*.ts", recursive: true);
        }

        allTsFiles = allTsFiles.Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
            !fileSystem.FileExists(x[..^".d.ts".Length] + ".ts"));

        files = files.Concat(allTsFiles.Where(x => includePatterns.Length > 0 &&
            includeGlob.IsMatch(x[(rootDir.Length + 1)..]) &&
            (excludePatterns.Length == 0 || !excludeGlob.IsMatch(x[(rootDir.Length + 1)..]))));

        cancellationToken.ThrowIfCancellationRequested();

        return files.Distinct().ToArray();
    }

    private static bool IsRelativeOrRooted(IFileSystem fileSystem, string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (path.StartsWith('.') ||
            path.StartsWith('/') ||
            path.StartsWith('\\') ||
            fileSystem.IsPathRooted(path))
            return true;

        return false;
    }

    public static TSConfig Read(IFileSystem fileSystem, string path)
    {
        var config = TryParseJsonFile<TSConfig>(fileSystem, path);
        var extends = PathHelper.ToPath(config?.Extends ?? "");
        var loop = 0;
        string basePath = path;
        while (!string.IsNullOrEmpty(extends) && loop++ < 10)
        {
            var baseDir = fileSystem.GetDirectoryName(basePath);
            basePath = fileSystem.Combine(baseDir, extends);
            var baseConfig = TryParseJsonFile<TSConfig>(fileSystem, basePath);

            if (baseConfig is null &&
                !IsRelativeOrRooted(fileSystem, extends) &&
                !fileSystem.FileExists(basePath))
            {
                do
                {
                    basePath = fileSystem.Combine(baseDir, "node_modules", extends);
                    baseConfig = TryParseJsonFile<TSConfig>(fileSystem, basePath);
                }
                while (baseConfig is null &&
                    !fileSystem.FileExists(basePath) &&
                    !string.IsNullOrEmpty(baseDir = fileSystem.GetDirectoryName(baseDir)));
            }

            if (baseConfig is null)
                break;

            config.Exclude ??= baseConfig.Exclude;
            config.Files ??= baseConfig.Files;
            config.Include ??= baseConfig.Include;
            config.RootDir ??= baseConfig.RootDir;
            
            if (baseConfig.CompilerOptions != null)
            {
                config.CompilerOptions ??= new();
                config.CompilerOptions.BaseUrl ??= baseConfig.CompilerOptions.BaseUrl;
                config.CompilerOptions.Module ??= baseConfig.CompilerOptions.Module;
                config.CompilerOptions.Paths ??= baseConfig.CompilerOptions.Paths;
                config.CompilerOptions.Types ??= baseConfig.CompilerOptions.Types;

                if (config.CompilerOptions.TypeRoots is null &&
                    baseConfig.CompilerOptions.TypeRoots is not null)
                {
                    // typeroots are relative to the base config files
                    var relativePath = fileSystem.GetRelativePath(fileSystem.GetDirectoryName(path), baseDir);
                    relativePath = PathHelper.ToUrl(relativePath);
                    config.CompilerOptions.TypeRoots = baseConfig.CompilerOptions.TypeRoots.Select(x => relativePath + "/" + x).ToArray();
                }
            }

            extends = baseConfig.Extends;
        }

        return config;
    }

    public static T TryParseJsonFile<T>(IFileSystem fileSystem, string path) where T: class
    {
        ArgumentExceptionHelper.ThrowIfNull(fileSystem);
        ArgumentExceptionHelper.ThrowIfNull(path);

        try
        {
            if (!fileSystem.FileExists(path))
                return null;

            var text = fileSystem.ReadAllText(path);

#if ISSOURCEGENERATOR
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, 
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore
                });
#else
            return JSON.ParseTolerant<T>(text);
#endif
        }
        catch
        {
            return null;
        }
    }

    public static void LocateTSConfigFiles(IFileSystem fileSystem,
        string projectDir, out string modulesPath, out string namespacesPath)
    {
        namespacesPath = null;
        modulesPath = null;

        foreach (var configPath in new[]
        {
            fileSystem.Combine(projectDir, "tsconfig.json"),
            fileSystem.Combine(projectDir, "Namespaces", "tsconfig.json"),
            fileSystem.Combine(projectDir, "Modules", "tsconfig.json")
        })
        {
            var config = Read(fileSystem, configPath);
            if (config is null)
                continue;

            if (config.CompilerOptions?.Module?.ToLowerInvariant() is not (null or "none"))
                modulesPath ??= configPath;
            else
                namespacesPath ??= configPath;

            if (modulesPath is not null &&
                namespacesPath is not null)
                break;
        }
    }
}