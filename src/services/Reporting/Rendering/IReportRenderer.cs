namespace Serenity.Reporting;

/// <summary>
/// Abstraction for a report renderer that renders a report.
/// </summary>
public interface IReportRenderer
{
    /// <summary>
    /// Renders a report.
    /// </summary>
    /// <param name="report">Report object</param>
    /// <param name="renderOptions">Report render options</param>
    /// <returns>The render result.</returns>
    public ReportRenderResult Render(IReport report, ReportRenderOptions renderOptions);
}