namespace Serenity.CodeGenerator;

/// <summary>
/// Thrown by <see cref="GeneratedFileWriter"/> when a file would be overwritten but
/// nobody can be asked (non-interactive mode) and overwriting was not allowed.
/// </summary>
public class GeneratedFileExistsException(string fileName)
    : InvalidOperationException($"File exists: {fileName}. " +
        "Use --overwrite to replace existing files in non-interactive mode.")
{
    /// <summary>
    /// Path of the file that already exists.
    /// </summary>
    public string FileName { get; } = fileName ??
        throw new ArgumentNullException(nameof(fileName));
}
