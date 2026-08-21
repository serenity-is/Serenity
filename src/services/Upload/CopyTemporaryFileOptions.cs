namespace Serenity.Web;

/// <summary>
/// Options for copying a temporary file to its target
/// </summary>
public class CopyTemporaryFileOptions : FormatFilenameOptions
{
    /// <summary>
    /// Gets or sets the temporary file.
    /// </summary>
    public string TemporaryFile { get; set; }

    /// <summary>
    /// Gets or sets the files to delete container.
    /// </summary>
    public IFilesToDelete FilesToDelete { get; set; }
}