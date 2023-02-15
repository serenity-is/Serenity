namespace Serenity.Reporting;

/// <summary>
/// Set of WKHTMLToPdf settings
/// </summary>
public class WKHtmlToPdfSettings
{
    /// <summary>
    /// Section key for WKHtmlToPdf
    /// </summary>
    public const string SectionKey = "WKHtmlToPdf";

    /// <summary>
    /// Gets/sets the wkhtmltopdf executable path
    /// </summary>
    public string UtilityExePath { get; set; }
}