namespace Serenity.Web;

/// <summary>
/// Options for <see cref="DiskUploadStorage"/>
/// </summary>
public class DiskUploadStorageOptions
{
    /// <summary>
    /// Gets or sets the root path.
    /// </summary>
    public string RootPath { get; set; }

    /// <summary>
    /// Gets or sets the root URL.
    /// </summary>
    public string RootUrl { get; set; }
}