using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Upload settings
/// </summary>
[DefaultSectionKey(SectionKey)]
public class UploadSettings : IOptions<UploadSettings>
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "UploadSettings";

    /// <summary>
    /// List of blacklisted extensions
    /// </summary>
    public string ExtensionBlacklist { get; set; } = ".;.asax;.compiled;.ascx;.asmx;.aspx;.bat;.cmd;.com;.config;.cshtml;" +
        ".dll;.jar;.jsp;.htm;.html;.htaccess;.htpasswd;.lnk;.php;.ps1;.vbe;.vbs";

    /// <summary>
    /// This allows including additional extensions in ExtensionBlacklist
    /// without having to write all the default extensions in ExtensionBlacklist
    /// </summary>
    public string ExtensionBlacklistInclude { get; set; }

    /// <summary>
    /// This allows excluding some extensions from ExtensionBlacklist
    /// without having to write all the default extensions in ExtensionBlacklist
    /// </summary>
    public string ExtensionBlacklistExclude { get; set; }

    /// <summary>
    /// List of whitelisted extensions; Even if an extension is in this list,
    /// it won't be allowed if it is also in the ExtensionBlacklist
    /// </summary>
    public string ExtensionWhitelist { get; set; } = ".3gp;.7z;.ai;.avi;.bmp;.csv;.doc;.docx;.eps;.jpg;.jpeg;.json;" +
        ".gif;.gz;.ico;.mpg;.mpeg;.mp3;.mp4;.mkv;.pdf;.png;.ppt;.pptx;.psd;" +
        ".rar;.rtf;.svg;.tif;.tiff;.txt;.wav;.webm;.webp;.xls;.xlsx;.xml;" +
        ".xps;.zip;";

    /// <summary>
    /// This allows including additional extensions to ExtensionWhitelist
    /// without overriding all the default extensions in ExtensionWhitelist
    /// </summary>
    public string ExtensionWhitelistInclude { get; set; }

    /// <summary>
    /// This allows excluding some extensions from ExtensionWhitelist
    /// without having to write all the default extensions in ExtensionWhitelist
    /// </summary>
    public string ExtensionWhitelistExclude { get; set; }

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