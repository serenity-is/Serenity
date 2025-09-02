namespace Serenity.Web;

/// <summary>
/// Options for <see cref="DiskUploadStorage"/>
/// </summary>
public class DiskUploadStorageOptions
{
    /// <summary>
    /// Root path
    /// </summary>
    public string RootPath { get; set; }

    /// <summary>
    /// Root URL
    /// </summary>
    public string RootUrl { get; set; }
}