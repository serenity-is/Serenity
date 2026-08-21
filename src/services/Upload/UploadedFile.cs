namespace Serenity.Services;

/// <summary>
/// Uploaded file item which is used by multiple upload editors
/// to store file information in a string field as JSON array.
/// </summary>
public class UploadedFile
{
    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    public string OriginalName { get; set; }
}