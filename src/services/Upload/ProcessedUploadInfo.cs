
namespace Serenity.Web;

/// <summary>
/// Information about a processed temporary upload
/// </summary>
public class ProcessedUploadInfo
{

    /// <summary>
    /// Gets or sets the image height.
    /// </summary>
    public int ImageHeight { get; set; }

    /// <summary>
    /// Gets or sets the image width.
    /// </summary>
    public int ImageWidth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file contains an image.
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// Gets or sets the file size.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the temporary file path.
    /// </summary>
    public string TemporaryFile { get; set; }

    /// <summary>
    /// Legacy error message. Always null.
    /// </summary>
    [Obsolete("This is always null, as we now throw exceptions on errors")]
    public string ErrorMessage { get; } = null;

    /// <summary>
    /// Legacy success flag. Always true.
    /// </summary>
    [Obsolete("This is always true, as we now throw exceptions on errors")]
    public bool Success { get; } = true;
}