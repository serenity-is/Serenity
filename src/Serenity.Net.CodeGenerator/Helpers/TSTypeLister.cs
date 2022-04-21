using Serenity.CodeGeneration;
using Serenity.IO;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace Serenity.CodeGenerator
{
    public class TSTypeLister
    {
        private readonly IFileSystem fileSystem;
        private readonly string projectDir;

        private readonly IPath Path;
        private readonly IFile File;
        private readonly IDirectory Directory;

        public TSTypeLister(IFileSystem fileSystem, string projectDir)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            Path = fileSystem.Path;
            File = fileSystem.File;
            Directory = fileSystem.Directory;
            this.projectDir = fileSystem.Path.GetFullPath(projectDir);
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
            var tsconfig = Path.Combine(projectDir, "tsconfig.json");
            IEnumerable<string> files = null;
            if (File.Exists(tsconfig))
            {
                var cfg = JsonSerializer.Deserialize<TSConfig>(File.ReadAllText(tsconfig),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                if (!cfg.Files.IsEmptyOrNull())
                {
                    files = cfg.Files.Where(x => File.Exists(Path.Combine(projectDir, PathHelper.ToPath(x))))
                        .Select(x => Path.GetFullPath(Path.Combine(projectDir, PathHelper.ToPath(x))));
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
                            return Path.Combine(projectDir, PathHelper.ToPath(s));
                        })
                        .Where(typeRoot => Directory.Exists(typeRoot))
                        .Select(typeRoot => Path.GetFullPath(typeRoot))
                        .SelectMany(typeRoot =>
                            Directory.GetDirectories(typeRoot)
                                .Where(typing => (cfg.CompilerOptions?.Types == null) || types.Contains(Path.GetDirectoryName(typing)))
                                .Where(typing => Path.GetFileName(typing).Contains("serenity", StringComparison.OrdinalIgnoreCase) ||
                                    !PathHelper.ToUrl(typing).Contains("/node_modules/", StringComparison.OrdinalIgnoreCase))
                                .Select(typing => Path.Combine(typing, "index.d.ts"))
                                .Where(typing => File.Exists(typing)))
                        .ToList();

                    var includePatterns = cfg.Include
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    var excludePatterns = (cfg.Exclude ?? Array.Empty<string>())
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    files = files.Concat(cfg.Include.Select(x => PathHelper.ToUrl(x))
                        .Where(x => x.StartsWith("../", StringComparison.Ordinal) &&
                        !x.Contains('*', StringComparison.Ordinal))
                        .Select(x => Path.Combine(projectDir, x))
                        .Select(x => PathHelper.ToPath(x))
                        .Where(x => File.Exists(x)));

                    var includeGlob = new GlobFilter(includePatterns);
                    var excludeGlob = new GlobFilter(excludePatterns);

                    var allTsFiles = Directory.GetFiles(projectDir, "*.ts", System.IO.SearchOption.AllDirectories)
                        .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                            !File.Exists(x.Substring(0, x.Length - ".d.ts".Length) + ".ts"));

                    files = files.Concat(allTsFiles.Where(x => includePatterns.Any() && 
                        includeGlob.IsMatch(x[(projectDir.Length + 1)..]) &&
                        (!excludePatterns.Any() || !excludeGlob.IsMatch(x[(projectDir.Length + 1)..]))));

                    files = files.Distinct();
                }
            }

            if (files == null || !files.Any())
            {
                var directories = new[]
                {
                    Path.Combine(projectDir, @"Modules"),
                    Path.Combine(projectDir, @"Imports"),
                    Path.Combine(projectDir, @"typings", "serenity"),
                    Path.Combine(projectDir, @"wwwroot", "Scripts", "serenity")
                }.Where(x => Directory.Exists(x));

                files = directories.SelectMany(x =>
                    Directory.GetFiles(x, "*.ts", System.IO.SearchOption.AllDirectories))
                    .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetFileName(x).StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetFileName(x).StartsWith("Serenity-", StringComparison.OrdinalIgnoreCase));

                var corelib = files.Where(x => string.Equals(Path.GetFileName(x),
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

            TSTypeListerAST typeListerAST = new(fileSystem);
            foreach (var file in files)
                typeListerAST.AddInputFile(file);

            var externalTypes = typeListerAST.ExtractTypes();
            return externalTypes;
        }
    }
}