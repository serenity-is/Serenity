
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
    /// <param name="reportKey">The report key, if it is not specified in report type as an attribute.
    /// Will be ignored if report will be rendered directly, e.g. not via a callback.</param>
    /// <param name="reportParams">The set of report params usually a serialized JSON object 
    /// to be passed to the URL callback. These params should be already be applied to the passed
    /// report instance. Will be ignored if the report is rendered directly, e.g. without a callback.</param>
    byte[] Render(IReport report, string reportKey, string reportParams);
}