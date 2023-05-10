namespace Serenity.Web;

/// <summary>
/// Information about an image format
/// </summary>
public class ImageFormatInfo
{
    /// <summary>
    /// Default mime type
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// List of expecteed file extensions
    /// </summary>
    public IEnumerable<string> FileExtensions { get; set; }
}