namespace Serenity.Reporting;

/// <summary>
/// Abstraction for the a report render that renders a report
/// </summary>
public interface IReportRenderer
{
    /// <summary>
    /// Renders a report
    /// </summary>
    /// <param name="report">Report object</param>
    /// <param name="options">Report render options</param>
    public ReportRenderResult Render(IReport report, ReportRenderOptions options);
}