namespace Serenity.Services;

/// <summary>
/// Uploaded file item which is used by multiple upload editors
/// to store file information in a string field as JSON array.
/// </summary>
public class UploadedFile
{
    /// <summary>
    /// File path
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string OriginalName { get; set; }
}