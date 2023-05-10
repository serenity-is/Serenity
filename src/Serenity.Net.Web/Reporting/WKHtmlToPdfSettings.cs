namespace Serenity.Reporting;

/// <summary>
/// Set of WKHTMLToPdf settings
/// </summary>
[DefaultSectionKey(SectionKey)]
public class WKHtmlToPdfSettings
{
    /// <summary>
    /// Section key for WKHtmlToPdf
    /// </summary>
    public const string SectionKey = "WKHtmlToPdf";

    /// <summary>
    /// Gets/sets the wkhtmltopdf executable path
    /// </summary>
    public string ExecutablePath { get; set; }
}