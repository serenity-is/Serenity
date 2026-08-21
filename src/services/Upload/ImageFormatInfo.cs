namespace Serenity.Web;

/// <summary>
/// Information about an image format
/// </summary>
public class ImageFormatInfo
{
    /// <summary>
    /// Gets or sets the default MIME type.
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Gets or sets the list of expected file extensions.
    /// </summary>
    public IEnumerable<string> FileExtensions { get; set; }
}