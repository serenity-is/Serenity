namespace Serenity.Web;

/// <summary>
/// Result of the copying of a temporary file to its target
/// </summary>
public class CopyTemporaryFileResult
{
    /// <summary>
    /// Gets or sets the path of the file.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the original name.
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file has a thumbnail.
    /// </summary>
    public bool HasThumbnail { get; set; }

    /// <summary>
    /// Gets or sets the file size.
    /// </summary>
    public long FileSize { get; set; }
}