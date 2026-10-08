namespace Serenity.CodeGenerator;

/// <summary>
/// Summarizes the file changes performed by <see cref="MultipleOutputHelper.WriteFiles"/>.
/// All paths are relative to the output directory (URL style, e.g. <c>Administration/UserRow.ts</c>).
/// </summary>
public class WriteFilesResult
{
    /// <summary>New files that were written.</summary>
    public List<string> Added { get; } = [];

    /// <summary>Existing files whose content was overwritten.</summary>
    public List<string> Modified { get; } = [];

    /// <summary>Stale files that were deleted.</summary>
    public List<string> Deleted { get; } = [];

    /// <summary>Stale files that would have been deleted, but were kept by safety checks.</summary>
    public List<string> SkippedDeletes { get; } = [];

    /// <summary>Files that would have been overwritten, but whose existing content was kept by safety checks.</summary>
    public List<string> SkippedUpdates { get; } = [];

    /// <summary>Number of files written (added + modified).</summary>
    public int WrittenCount => Added.Count + Modified.Count;

    /// <summary>Number of files deleted.</summary>
    public int DeletedCount => Deleted.Count;

    /// <summary>True if any file was added, modified or deleted.</summary>
    public bool HasChanges => Added.Count > 0 || Modified.Count > 0 || Deleted.Count > 0;

    /// <summary>True if any change was suppressed by safety checks.</summary>
    public bool HasSkippedChanges => SkippedDeletes.Count > 0 || SkippedUpdates.Count > 0;
}

public class MultipleOutputHelper
{
    private static readonly Encoding utf8 = new UTF8Encoding(true);

    public static WriteFilesResult WriteFiles(IFileSystem fileSystem,
#if !ISSOURCEGENERATOR
        IGeneratorConsole console,
#endif
        string outDir, IEnumerable<(string Path, string Text)> filesToWrite,
        string[]? deleteExtraPattern,
        string? endOfLine,
        bool designTimeBuild = false,
        string[]? designTimeCoreFiles = null,
        bool allowDeletion = true,
        IEnumerable<string>? preservedFiles = null,
        bool collectSkippedDeletes = false)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        var result = new WriteFilesResult();

        outDir = fileSystem.GetFullPath(outDir);
        fileSystem.CreateDirectory(outDir);

        var outputFiles = filesToWrite.ToArray();
        var generated = new HashSet<string>(outputFiles.Select(x => PathHelper.ToUrl(x.Path)),
            StringComparer.OrdinalIgnoreCase);
        var extraPatterns = deleteExtraPattern ?? [];
        var outRoot = PathHelper.ToUrl(outDir).TrimEnd('/') + '/';

        // Files kept as-is even though the generated content differs (e.g. design-time
        // snapshots that would collapse a populated core file to an empty baseline).
        // They are treated as generated (so they are not deleted) and never overwritten.
        var preservedSet = new HashSet<string>(
            (preservedFiles ?? []).Where(x => !string.IsNullOrEmpty(x))
                .Select(x => PathHelper.ToUrl(x)!), StringComparer.OrdinalIgnoreCase);
        foreach (var preserved in preservedSet)
        {
            generated.Add(preserved);
            if (fileSystem.FileExists(fileSystem.Combine(outDir, preserved)))
                result.SkippedUpdates.Add(preserved);
        }

        foreach (var (path, txt) in outputFiles)
        {
            var outFile = fileSystem.Combine(outDir, path);
            bool exists = fileSystem.FileExists(outFile);
            if (exists && preservedSet.Contains(PathHelper.ToUrl(path)!))
                continue;

            if (exists)
            {
                var content = fileSystem.ReadAllText(outFile, utf8);
                if (content.Trim().Replace("\r", "", StringComparison.Ordinal) ==
                    (txt ?? "").Trim().Replace("\r", "", StringComparison.Ordinal))
                    continue;
            }
            else if (!fileSystem.DirectoryExists(fileSystem.GetDirectoryName(outFile)))
                fileSystem.CreateDirectory(fileSystem.GetDirectoryName(outFile));

#if !ISSOURCEGENERATOR
            console.Write(exists ? "Overwriting: " : "New File: ", 
                exists ? ConsoleColor.Magenta : ConsoleColor.Green);
            console.WriteLine(fileSystem.GetFileName(outFile));
#endif

            string text = txt ?? "";
            if (string.Equals(endOfLine, "lf", StringComparison.OrdinalIgnoreCase))
                text = text.Replace("\r", "");
            else if (string.Equals(endOfLine, "crlf", StringComparison.OrdinalIgnoreCase))
                text = text.Replace("\r", "").Replace("\n", "\r\n");

            fileSystem.WriteAllText(outFile, text, utf8);

            if (exists)
                result.Modified.Add(PathHelper.ToUrl(path));
            else
                result.Added.Add(PathHelper.ToUrl(path));
        }

        if (extraPatterns.Length == 0)
            return result;

        string relativeOf(string file) => PathHelper.ToUrl(file)[outRoot.Length..];

        string[] computeStaleFiles()
        {
            return extraPatterns.SelectMany(x => fileSystem.GetFiles(outDir, x, recursive: true))
                .Distinct()
                .Where(file =>
                {
                    var filePath = PathHelper.ToUrl(file);
                    return filePath.StartsWith(outRoot, StringComparison.Ordinal) &&
                        !generated.Contains(filePath[outRoot.Length..]);
                })
                .ToArray();
        }

        // Keep stale outputs whenever deletion is not authorized (compiler errors or a
        // collapsed/incomplete compilation). This applies in every context, not only
        // design-time builds, because VS also runs real builds with partial compilations
        // (e.g. BuildingProject=true with a nearly empty syntax tree set while loading).
        // Only enumerate the stale candidates when they are actually going to be reported,
        // to avoid an extra directory scan when tracing/diagnostics are disabled.
        if (!allowDeletion)
        {
            if (collectSkippedDeletes)
                result.SkippedDeletes.AddRange(computeStaleFiles().Select(relativeOf));
            return result;
        }

        var filesToDelete = computeStaleFiles();

        if (designTimeBuild)
        {
            var coreFiles = new HashSet<string>((designTimeCoreFiles ?? [])
                .Select(x => PathHelper.ToUrl(x)!), StringComparer.OrdinalIgnoreCase);
            var nonCoreFilesToDelete = filesToDelete.Where(file =>
            {
                var filePath = PathHelper.ToUrl(file);
                return !coreFiles.Contains(filePath[outRoot.Length..]);
            }).ToArray();
            var generatedNonCoreFileCount = generated.Count(file => !coreFiles.Contains(file));

            // Allow one stale type file to be deleted when other non-core types were generated, so intentional
            // removals and renames are reflected during design-time builds. Multiple stale files or no generated
            // non-core types may indicate incomplete discovery, so preserve existing outputs in those cases.
            if (nonCoreFilesToDelete.Length != 1 || generatedNonCoreFileCount == 0)
            {
                result.SkippedDeletes.AddRange(filesToDelete.Select(relativeOf));
                return result;
            }

            var nonCoreSet = new HashSet<string>(nonCoreFilesToDelete
                .Select(x => PathHelper.ToUrl(x)!), StringComparer.OrdinalIgnoreCase);
            result.SkippedDeletes.AddRange(filesToDelete
                .Where(file => !nonCoreSet.Contains(PathHelper.ToUrl(file)))
                .Select(relativeOf));
            filesToDelete = nonCoreFilesToDelete;
        }

        foreach (var file in filesToDelete)
        {
#if !ISSOURCEGENERATOR
            console.Write("Deleting: ", ConsoleColor.Yellow);
            console.WriteLine(fileSystem.GetFileName(file));
#endif
            fileSystem.DeleteFile(file);
            result.Deleted.Add(relativeOf(file));
        }

        return result;
    }
}
