namespace Serenity.CodeGenerator;

public class MultipleOutputHelper
{
    private static readonly Encoding utf8 = new UTF8Encoding(true);

    public static void WriteFiles(IFileSystem fileSystem,
#if !ISSOURCEGENERATOR
        IGeneratorConsole console,
#endif
        string outDir, IEnumerable<(string Path, string Text)> filesToWrite,
        string[]? deleteExtraPattern,
        string? endOfLine,
        bool designTimeBuild = false,
        string[]? designTimeCoreFiles = null,
        bool allowDesignTimeDeletion = true)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        outDir = fileSystem.GetFullPath(outDir);
        fileSystem.CreateDirectory(outDir);

        var outputFiles = filesToWrite.ToArray();
        var generated = new HashSet<string>(outputFiles.Select(x => PathHelper.ToUrl(x.Path)),
            StringComparer.OrdinalIgnoreCase);
        var extraPatterns = deleteExtraPattern ?? [];
        var outRoot = PathHelper.ToUrl(outDir).TrimEnd('/') + '/';

        foreach (var (path, txt) in outputFiles)
        {
            var outFile = fileSystem.Combine(outDir, path);
            bool exists = fileSystem.FileExists(outFile);
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
        }

        if (extraPatterns.Length == 0)
            return;

        // Keep stale outputs while compiler errors can temporarily hide generated types.
        if (designTimeBuild && !allowDesignTimeDeletion)
            return;

        var filesToDelete = extraPatterns.SelectMany(x => fileSystem.GetFiles(outDir, x, recursive: true))
            .Distinct()
            .Where(file =>
            {
                var filePath = PathHelper.ToUrl(file);
                return filePath.StartsWith(outRoot, StringComparison.Ordinal) &&
                    !generated.Contains(filePath[outRoot.Length..]);
            })
            .ToArray();

        if (designTimeBuild)
        {
            var coreFiles = new HashSet<string>((designTimeCoreFiles ?? [])
                .Select(PathHelper.ToUrl).OfType<string>(), StringComparer.OrdinalIgnoreCase);
            var nonCoreFilesToDelete = filesToDelete.Where(file =>
            {
                var filePath = PathHelper.ToUrl(file);
                return !coreFiles.Contains(filePath[outRoot.Length..]);
            }).ToArray();
            var generatedNonCoreFileCount = generated.Count(file => !coreFiles.Contains(file));

            // A single stale type file is a likely intentional edit; multiple missing files may indicate partial discovery.
            if (nonCoreFilesToDelete.Length != 1 || generatedNonCoreFileCount == 0)
                return;

            filesToDelete = nonCoreFilesToDelete;
        }

        foreach (var file in filesToDelete)
        {
#if !ISSOURCEGENERATOR
            console.Write("Deleting: ", ConsoleColor.Yellow);
            console.WriteLine(fileSystem.GetFileName(file));
#endif
            fileSystem.DeleteFile(file);
        }
    }
}