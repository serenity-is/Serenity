using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Upload settings
/// </summary>
public class UploadSettings : IOptions<UploadSettings>
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "UploadSettings";

    /// <summary>
    /// Root path for uploads, default is "App_Data/upload/"
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Root ur for uploads, default is "~/upload"
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public UploadSettings()
    {
        Path = "App_Data/upload/";
        Url = "~/upload/";
    }

    /// <summary>
    /// Gets this instance
    /// </summary>
    public UploadSettings Value => this;
}