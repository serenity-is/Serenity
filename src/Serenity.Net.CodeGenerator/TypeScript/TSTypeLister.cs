using System.Threading;
#if !ISSOURCEGENERATOR
using Serenity.CodeGeneration;
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

        public List<ExternalType> List(out bool isMixedModules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            isMixedModules = false;
            IEnumerable<string> files = null;
            var rootConfig = TSConfigHelper.Read(fileSystem, fileSystem.Combine(projectDir, "tsconfig.json"));
            if (rootConfig is not null)
            {
                files = TSConfigHelper.ListFiles(rootConfig, fileSystem, projectDir, cancellationToken);
                if (rootConfig.CompilerOptions?.Module is null or "none")
                {
                    var modularConfigPath = fileSystem.Combine(projectDir, "Modules", "tsconfig.json");
                    var modularConfig = TSConfigHelper.Read(fileSystem, modularConfigPath);
                    if (modularConfig is not null && modularConfig.CompilerOptions?.Module is not null or "none")
                    {
                        files = (files ?? Array.Empty<string>()).Concat(
                                TSConfigHelper.ListFiles(modularConfig, fileSystem, projectDir, cancellationToken))
                            .Distinct();
                        isMixedModules = true;
                    }
                }
            }

            if (files == null || !files.Any())
            {
                // legacy apps
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