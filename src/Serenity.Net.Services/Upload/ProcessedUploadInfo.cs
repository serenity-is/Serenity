
namespace Serenity.Web;

/// <summary>
/// Information about a processed temporary upload
/// </summary>
public class ProcessedUploadInfo
{
    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Image height
    /// </summary>
    public int ImageHeight { get; set; }

    /// <summary>
    /// Image width
    /// </summary>
    public int ImageWidth { get; set; }

    /// <summary>
    /// True if the file contains an image
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// File size
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// True if upload and all validations were successfull
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Temporary file path
    /// </summary>
    public string TemporaryFile { get; set; }
}