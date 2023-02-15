
namespace Serenity.Reporting;

/// <summary>
/// PDF exporter interface for HTML reports.
/// </summary>
public interface IHtmlReportPdfRenderer
{
    /// <summary>
    /// Renders the specified report to PDF.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="key">The report key, if it is not specified in report type as an attribute.
    /// Will be ignored if report will be rendered directly, e.g. not via a callback.</param>
    /// <param name="options">The set of options to be passed to the URL callback.
    /// Will not be used if report will be rendered directly.</param>
    byte[] Render(IReport report, string key, string options);
}