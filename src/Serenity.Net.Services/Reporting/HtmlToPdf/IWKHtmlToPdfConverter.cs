namespace Serenity.Reporting;

/// <summary>
/// WKHTMLToPdf converter interface
/// </summary>
public interface IWKHtmlToPdfConverter : IHtmlToPdfConverter
{
    /// <summary>
    /// Gets the path to the wkhtmltopdf executable
    /// </summary>
    string GetExecutablePath();
}