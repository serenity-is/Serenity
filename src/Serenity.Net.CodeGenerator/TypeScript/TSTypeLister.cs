using Serenity.IO;
using System.Threading;
#if ISSOURCEGENERATOR
using Newtonsoft.Json;
#else
using Serenity.CodeGeneration;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
#endif

namespace Serenity.CodeGenerator
{
    public class TSTypeLister
    {
        private readonly IGeneratorFileSystem fileSystem;
        private readonly CancellationToken cancellationToken;
        private readonly string projectDir;

        public TSTypeLister(IGeneratorFileSystem fileSystem, string projectDir,
            CancellationToken cancellationToken = default)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.cancellationToken = cancellationToken;
            this.projectDir = fileSystem.GetFullPath(projectDir);
        }

        private class TSConfig
        {
            public CompilerConfig CompilerOptions { get; set; }
            public string[] Files { get; set; }
            public string[] Include { get; set; }
            public string[] Exclude { get; set; }

            public class CompilerConfig
            {
                public string[] TypeRoots { get; set; }
                public string[] Types { get; set; }
            }
        }

        public List<ExternalType> List()
        {
            var tsconfig = fileSystem.Combine(projectDir, "tsconfig.json");
            IEnumerable<string> files = null;
            cancellationToken.ThrowIfCancellationRequested();
            
            if (fileSystem.FileExists(tsconfig))
            {
#if ISSOURCEGENERATOR
                var cfg = JsonConvert.DeserializeObject<TSConfig>(fileSystem.ReadAllText(tsconfig), new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
#else
                var cfg = JsonSerializer.Deserialize<TSConfig>(fileSystem.ReadAllText(tsconfig),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
#endif
                if (!cfg.Files.IsEmptyOrNull())
                {
                    files = cfg.Files.Where(x => fileSystem.FileExists(fileSystem.Combine(projectDir, PathHelper.ToPath(x))))
                        .Select(x => fileSystem.GetFullPath(fileSystem.Combine(projectDir, PathHelper.ToPath(x))));
                }
                else if (!cfg.Include.IsEmptyOrNull())
                {
                    var typeRoots = cfg.CompilerOptions?.TypeRoots?.IsEmptyOrNull() != false ?
                        new string[] { "./node_modules/@types" } : cfg.CompilerOptions.TypeRoots;

                    var types = new HashSet<string>(cfg.CompilerOptions?.Types ?? Array.Empty<string>(),
                        StringComparer.OrdinalIgnoreCase);

                    files = typeRoots.Select(typeRoot =>
                        {
                            var s = PathHelper.ToUrl(typeRoot);
                            if (s.StartsWith("./", StringComparison.Ordinal))
                                s = s[2..];
                            return fileSystem.Combine(projectDir, PathHelper.ToPath(s));
                        })
                        .Where(typeRoot => fileSystem.DirectoryExists(typeRoot))
                        .Select(typeRoot => fileSystem.GetFullPath(typeRoot))
                        .SelectMany(typeRoot =>
                            fileSystem.GetDirectories(typeRoot)
                                .Where(typing => (cfg.CompilerOptions?.Types == null) || types.Contains(fileSystem.GetDirectoryName(typing)))
                                .Where(typing => fileSystem.GetFileName(typing).Contains("serenity", StringComparison.OrdinalIgnoreCase) ||
                                    !PathHelper.ToUrl(typing).Contains("/node_modules/", StringComparison.OrdinalIgnoreCase))
                                .Select(typing => fileSystem.Combine(typing, "index.d.ts"))
                                .Where(typing => fileSystem.FileExists(typing)))
                        .ToList();

                    cancellationToken.ThrowIfCancellationRequested();

                    var includePatterns = cfg.Include
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    cancellationToken.ThrowIfCancellationRequested();

                    var excludePatterns = (cfg.Exclude ?? Array.Empty<string>())
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    cancellationToken.ThrowIfCancellationRequested();

                    files = files.Concat(cfg.Include.Select(x => PathHelper.ToUrl(x))
                        .Where(x => x.StartsWith("../", StringComparison.Ordinal) &&
                        !x.Contains('*', StringComparison.Ordinal))
                        .Select(x => fileSystem.Combine(projectDir, x))
                        .Select(x => PathHelper.ToPath(x))
                        .Where(x => fileSystem.FileExists(x)));

                    cancellationToken.ThrowIfCancellationRequested();

                    var includeGlob = new GlobFilter(includePatterns);
                    var excludeGlob = new GlobFilter(excludePatterns);

                    var allTsFiles = fileSystem.GetFiles(projectDir, "*.ts", recursive: true)
                        .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                            !fileSystem.FileExists(x[..^".d.ts".Length] + ".ts"));

                    files = files.Concat(allTsFiles.Where(x => includePatterns.Any() &&
                        includeGlob.IsMatch(x[(projectDir.Length + 1)..]) &&
                        (!excludePatterns.Any() || !excludeGlob.IsMatch(x[(projectDir.Length + 1)..]))));

                    cancellationToken.ThrowIfCancellationRequested();

                    files = files.Distinct();
                }
            }

            if (files == null || !files.Any())
            {
                var directories = new[]
                {
                    fileSystem.Combine(projectDir, @"Modules"),
                    fileSystem.Combine(projectDir, @"Imports"),
                    fileSystem.Combine(projectDir, @"typings", "serenity"),
                    fileSystem.Combine(projectDir, @"wwwroot", "Scripts", "serenity")
                }.Where(x => fileSystem.DirectoryExists(x));

                cancellationToken.ThrowIfCancellationRequested();

                files = directories.SelectMany(x =>
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

                files = files.OrderBy(x => x);
            }

            TSTypeListerAST typeListerAST = new(fileSystem, cancellationToken);
            foreach (var file in files)
                typeListerAST.AddInputFile(file);

            var externalTypes = typeListerAST.ExtractTypes();
            return externalTypes;
        }
    }
}