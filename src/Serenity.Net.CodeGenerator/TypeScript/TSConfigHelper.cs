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
            return LegacyListFiles(fileSystem, configPath, cancellationToken);

        return ListFiles(tsConfig, fileSystem, fileSystem.GetDirectoryName(configPath), cancellationToken);
    }

    private static IEnumerable<string> ListFiles(TSConfig config,
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

        var includePatterns = config.Include;
        if (includePatterns == null)
        {
            if (config.Exclude == null || config.Exclude.Length == 0)
                return LegacyListFiles(fileSystem, 
                    fileSystem.Combine(rootDir, "tsconfig.json"), cancellationToken);

            includePatterns = ["**/*"];
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

        files = files.Concat(GetFilesMatchingPatterns(fileSystem, rootDir, includePatterns, config?.Exclude));

        cancellationToken.ThrowIfCancellationRequested();

        return files.Distinct().ToArray();
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

    public static T TryParseJsonFile<T>(IFileSystem fileSystem, string path) where T : class
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

    private static readonly char[] wildcards = ['*', '?'];

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

    private static string NormalizePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return "";

        pattern = PathHelper.ToUrl(pattern);
        if (pattern.StartsWith("./", StringComparison.Ordinal))
            pattern = pattern[2..];

        if (pattern.EndsWith('/'))
            pattern += "**/*";
        else
        {
            var filename = System.IO.Path.GetFileName(pattern);
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(filename)) &&
                filename.IndexOfAny(wildcards) < 0)
                pattern += "/**/*";
        }

        return pattern;
    }

    private static IEnumerable<string> GetFilesMatchingPatterns(IFileSystem fileSystem,
        string rootDir, string[] includePatterns, string[] excludePatterns)
    {
        List<string> result = [];

        includePatterns = includePatterns.Select(NormalizePattern)
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

        excludePatterns = (excludePatterns ?? []).Select(NormalizePattern)
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

        includePatterns = includePatterns.Where(x =>
        {
            if (x.IndexOfAny(wildcards) < 0 ||
                x.StartsWith('/') ||
                x == ".." ||
                x.Contains("../", StringComparison.Ordinal) ||
                x.Contains("..\\", StringComparison.Ordinal) &&
                x.Contains(':', StringComparison.Ordinal))
            {
                var path = fileSystem.Combine(rootDir, PathHelper.ToPath(x));
                if (fileSystem.FileExists(path))
                    result.Add(path);
                return false;
            }
            return true;
        }).ToArray();

        IEnumerable<string> enumerated = [];

        var scanFolders = includePatterns.Select(x => x.Split('/')[0])
            .Where(x => !string.IsNullOrEmpty(x) &&
                x.IndexOfAny(wildcards) < 0)
            .ToArray();

        if (scanFolders.Length == includePatterns.Length)
        {
            var scanFoldersSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in scanFolders)
                scanFoldersSet.Add(s);

            enumerated = enumerated.Concat(fileSystem.GetFiles(rootDir, "*.ts"));
            enumerated = enumerated.Concat(fileSystem.GetFiles(rootDir, "*.tsx"));

            foreach (var directory in fileSystem.GetDirectories(rootDir))
            {
                var directoryName = fileSystem.GetFileName(directory);
                if (!scanFoldersSet.Contains(directoryName))
                    continue;

                enumerated = enumerated.Concat(fileSystem.GetFiles(directory, "*.ts", recursive: true));
                enumerated = enumerated.Concat(fileSystem.GetFiles(directory, "*.tsx", recursive: true));
            }
            enumerated = enumerated.ToArray();
        }
        else
        {
            enumerated = fileSystem.GetFiles(rootDir, "*.ts", recursive: true);
        }

        enumerated = enumerated.Where(x =>
            !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
            !fileSystem.FileExists(x[..^".d.ts".Length] + ".ts"));

        var includeGlob = new IO.GlobFilter(includePatterns.Select(x => '/' + x));
        var excludeGlob = new IO.GlobFilter(excludePatterns.Select(x => '/' + x));

        enumerated = enumerated.Where(x => includePatterns.Length > 0 &&
            includeGlob.IsMatch(fileSystem.GetRelativePath(rootDir, x)) &&
            (excludePatterns.Length == 0 || 
             !excludeGlob.IsMatch(fileSystem.GetRelativePath(rootDir, x))));

        return result.Concat(enumerated);
    }

    private static IEnumerable<string> LegacyListFiles(IFileSystem fileSystem, 
        string tsConfigPath, CancellationToken cancellationToken)
    {
        // legacy apps
        var projectDir = fileSystem.GetDirectoryName(tsConfigPath);
        var directories = new[]
        {
                fileSystem.Combine(projectDir, @"Modules"),
                fileSystem.Combine(projectDir, @"Imports"),
                fileSystem.Combine(projectDir, @"typings", "serenity"),
                fileSystem.Combine(projectDir, @"wwwroot", "Scripts", "serenity")
            }.Where(x => fileSystem.DirectoryExists(x));

        cancellationToken.ThrowIfCancellationRequested();

        var files = directories.SelectMany(x =>
            fileSystem.GetFiles(x, "*.ts", recursive: true))
            .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                fileSystem.GetFileName(x).StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) ||
                fileSystem.GetFileName(x).StartsWith("Serenity-", StringComparison.OrdinalIgnoreCase));

        cancellationToken.ThrowIfCancellationRequested();

        var corelib = files.Where(x => string.Equals(fileSystem.GetFileName(x),
            "Serenity.CoreLib.d.ts", StringComparison.OrdinalIgnoreCase));

        static bool corelibUnderTypings(string x) =>
            x.Replace('\\', '/').EndsWith("/typings/serenity/Serenity.CoreLib.d.ts",
                StringComparison.OrdinalIgnoreCase);

        if (corelib.Count() > 1 &&
            corelib.Any(x => !corelibUnderTypings(x)))
        {
            files = files.Where(x => !corelibUnderTypings(x));
        }

        return files.OrderBy(x => x);
    }
}