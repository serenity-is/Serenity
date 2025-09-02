namespace Serenity.Web;

/// <summary>
/// Result of the copying of a temporary file to its target
/// </summary>
public class CopyTemporaryFileResult
{
    /// <summary>
    /// The path of the file
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Original name
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// If the file has thumbnail
    /// </summary>
    public bool HasThumbnail { get; set; }

    /// <summary>
    /// File size
    /// </summary>
    public long FileSize { get; set; }
}