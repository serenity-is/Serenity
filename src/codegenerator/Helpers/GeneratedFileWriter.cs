namespace Serenity.CodeGenerator;

public class GeneratedFileWriter(IFileSystem fileSystem, IGeneratorConsole console) : IGeneratedFileWriter
{
    private static readonly UTF8Encoding utf8 = new(true);
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IGeneratorConsole console = console ?? throw new ArgumentNullException(nameof(console));

    /// <summary>
    /// What to do when a target file exists with different content. Defaults to
    /// <see cref="OverwriteMode.Ask"/>, i.e. prompt the user.
    /// </summary>
    public OverwriteMode Overwrite { get; set; } = OverwriteMode.Ask;

    private bool? overwriteAll;

    public static byte[] ToUTF8BOM(string s)
    {
        return [.. Encoding.UTF8.GetPreamble(), .. utf8.GetBytes(s)];
    }

    public void WriteAllText(string targetFile, string contents)
    {
        if (fileSystem.FileExists(targetFile))
        {
            var existingContent = fileSystem.ReadAllText(targetFile, utf8);

            if (existingContent.Trim().Replace("\r", "", StringComparison.Ordinal) ==
                contents.Trim().Replace("\r", "", StringComparison.Ordinal))
                return;

            switch (Overwrite)
            {
                case OverwriteMode.Skip:
                    return;

                case OverwriteMode.Fail:
                    throw new GeneratedFileExistsException(targetFile);

                case OverwriteMode.Backup:
                    var backupFile = string.Format(CultureInfo.InvariantCulture,
                        "{0}.{1}.bak", targetFile, DateTime.Now.ToString(
                        "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));

                    fileSystem.WriteAllBytes(backupFile,
                        fileSystem.ReadAllBytes(targetFile));
                    break;

                case OverwriteMode.Force:
                    break;

                default:
                    if (!ConfirmOverwrite(targetFile))
                        return;
                    break;
            }
        }
        else
            fileSystem.CreateDirectory(fileSystem.GetDirectoryName(targetFile));

        fileSystem.WriteAllBytes(targetFile, ToUTF8BOM(contents));
    }

    private bool ConfirmOverwrite(string targetFile)
    {
        if (overwriteAll != null)
            return overwriteAll.Value;

        string? answer;
        while (true)
        {
            console.Write("Overwrite " + fileSystem.GetFileName(targetFile) + 
                "? ([Y]es, [N]o, Yes to [A]ll, [S]kip All): ");
            answer = console.ReadLine();
            
            if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(answer, "n", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(answer, "no", StringComparison.OrdinalIgnoreCase))
                return false;

            if (string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
            {
                overwriteAll = false;
                return false;
            }

            if (string.Equals(answer, "a", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(answer, "all", StringComparison.OrdinalIgnoreCase))
            {
                overwriteAll = true;
                return true;
            }
        }
    }
}