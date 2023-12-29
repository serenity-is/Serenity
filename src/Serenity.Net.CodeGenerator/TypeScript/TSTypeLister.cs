using Serenity.TypeScript;
using System.Threading;
#if ISSOURCEGENERATOR
using System.Collections.Concurrent;
#else
using Serenity.CodeGeneration;
#endif

namespace Serenity.CodeGenerator;

internal class TSTypeLister
{
    private readonly IFileSystem fileSystem;
    private readonly CancellationToken cancellationToken;
    private readonly string tsConfigPath;
    private readonly ConcurrentDictionary<string, object> astCache;

    public TSTypeLister(IFileSystem fileSystem, string tsConfigPath, ConcurrentDictionary<string, object> astCache = null,
        CancellationToken cancellationToken = default)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.astCache = astCache;
        this.cancellationToken = cancellationToken;
        ArgumentExceptionHelper.ThrowIfNull(tsConfigPath);

        this.tsConfigPath = fileSystem.GetFullPath(tsConfigPath);
    }

    public List<ExternalType> List()
    {
        cancellationToken.ThrowIfCancellationRequested();

        var files = TSConfigHelper.ListFiles(fileSystem, tsConfigPath, 
            out var tsConfig, cancellationToken);

        TSTypeListerAST typeListerAST = new(fileSystem,
            tsConfigDir: fileSystem.GetDirectoryName(tsConfigPath), tsConfig: tsConfig,
            astCache, cancellationToken);

        foreach (var file in files)
            typeListerAST.AddInputFile(file);

        var externalTypes = typeListerAST.ExtractTypes();
        return externalTypes;
    }
}