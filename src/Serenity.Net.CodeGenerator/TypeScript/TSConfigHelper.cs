using System.Threading;
#if ISSOURCEGENERATOR
using Newtonsoft.Json;
#else
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
#endif

namespace Serenity.CodeGenerator
{
    public static class TSConfigHelper
    {
        public static IEnumerable<string> ListFiles(
            IGeneratorFileSystem fileSystem, string configPath, out TSConfig tsConfig,
            CancellationToken cancellationToken = default)
        {
            tsConfig = Read(fileSystem, configPath);
            if (tsConfig is null)
                return null;

            return ListFiles(tsConfig, fileSystem, fileSystem.GetDirectoryName(configPath), cancellationToken);
        }

        public static IEnumerable<string> ListFiles(TSConfig config,
            IGeneratorFileSystem fileSystem, string rootDir,
            CancellationToken cancellationToken = default)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

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

                include = new string[] { "**/*" };
            }

            var typeRoots = config.CompilerOptions?.TypeRoots?.IsEmptyOrNull() != false ?
                new string[] { "./node_modules/@types" } : config.CompilerOptions.TypeRoots;

            var types = new HashSet<string>(config.CompilerOptions?.Types ??
                Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            IEnumerable<string> files = typeRoots.Select(typeRoot =>
            {
                var s = PathHelper.ToUrl(typeRoot);
                if (s.StartsWith("./", StringComparison.Ordinal))
                    s = s[2..];
                return fileSystem.Combine(rootDir, PathHelper.ToPath(s));
            })
            .Where(typeRoot => fileSystem.DirectoryExists(typeRoot))
            .Select(typeRoot => fileSystem.GetFullPath(typeRoot))
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
                .Select(x => PathHelper.ToUrl(x))
                .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                    (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                .Select(x => PathHelper.ToPath(x));

            cancellationToken.ThrowIfCancellationRequested();

            var excludePatterns = (config.Exclude ?? Array.Empty<string>())
                .Select(x => PathHelper.ToUrl(x))
                .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                    (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                .Select(x => PathHelper.ToPath(x));

            cancellationToken.ThrowIfCancellationRequested();

            files = files.Concat(config.Include.Select(x => PathHelper.ToUrl(x))
                .Where(x => x.StartsWith("../", StringComparison.Ordinal) &&
                !x.Contains('*', StringComparison.Ordinal))
                .Select(x => fileSystem.Combine(rootDir, x))
                .Select(x => PathHelper.ToPath(x))
                .Where(x => fileSystem.FileExists(x)));

            cancellationToken.ThrowIfCancellationRequested();

            var includeGlob = new IO.GlobFilter(includePatterns);
            var excludeGlob = new IO.GlobFilter(excludePatterns);

            var allTsFiles = fileSystem.GetFiles(rootDir, "*.ts", recursive: true)
                .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                    !fileSystem.FileExists(x[..^".d.ts".Length] + ".ts"));

            files = files.Concat(allTsFiles.Where(x => includePatterns.Any() &&
                includeGlob.IsMatch(x[(rootDir.Length + 1)..]) &&
                (!excludePatterns.Any() || !excludeGlob.IsMatch(x[(rootDir.Length + 1)..]))));

            cancellationToken.ThrowIfCancellationRequested();

            return files.Distinct().ToArray();
        }

        public static TSConfig Read(IGeneratorFileSystem fileSystem, string path)
        {
            return TryParseJsonFile<TSConfig>(fileSystem, path);
        }

        public static T TryParseJsonFile<T>(IGeneratorFileSystem fileSystem, string path) where T: class
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                if (!fileSystem.FileExists(path))
                    return null;

                var text = fileSystem.ReadAllText(path);

#if ISSOURCEGENERATOR
                return JsonConvert.DeserializeObject<T>(text, 
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
#else
                return JsonSerializer.Deserialize<T>(text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
#endif
            }
            catch
            {
                return null;
            }
        }

        public static void LocateTSConfigFiles(IGeneratorFileSystem fileSystem,
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

                if (config.CompilerOptions?.Module is not (null or "none"))
                    modulesPath ??= configPath;
                else
                    namespacesPath ??= configPath;

                if (modulesPath is not null &&
                    namespacesPath is not null)
                    break;
            }
        }
    }
}