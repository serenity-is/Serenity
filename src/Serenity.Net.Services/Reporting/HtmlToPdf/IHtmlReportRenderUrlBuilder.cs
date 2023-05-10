namespace Serenity.Reporting;

/// <summary>
/// Interface abstraction to get HTML report render URL
/// </summary>
public interface IHtmlReportRenderUrlBuilder
{
    /// <summary>
    /// Gets the render URL for the specified report. The response object implements IDisposable.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">Report render options</param>
    HtmlReportRenderUrl GetRenderUrl(IReport report, ReportRenderOptions options);
}