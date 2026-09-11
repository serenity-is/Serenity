
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
    /// <param name="renderOptions">Report render options</param>
    byte[] Render(IReport report, ReportRenderOptions renderOptions);
}