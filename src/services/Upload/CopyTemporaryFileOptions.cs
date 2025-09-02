namespace Serenity.Web;

/// <summary>
/// Options for copying a temporary file to its target
/// </summary>
public class CopyTemporaryFileOptions : FormatFilenameOptions
{
    /// <summary>
    /// Temporary file
    /// </summary>
    public string TemporaryFile { get; set; }

    /// <summary>
    /// Files to delete container
    /// </summary>
    public IFilesToDelete FilesToDelete { get; set; }
}