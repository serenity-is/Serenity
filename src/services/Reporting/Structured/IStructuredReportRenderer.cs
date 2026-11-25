namespace Serenity.Reporting;

/// <summary>
/// Renders an <see cref="IStructuredReport"/> to a file, e.g. an Excel workbook with a sheet for each of its tables.
/// The default report renderer uses it, if one is registered, for the structured reports.
/// </summary>
public interface IStructuredReportRenderer
{
    /// <summary>
    /// Renders a structured report in the export format of the options, e.g. "xlsx", "csv" or "pdf".
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">The render options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The render result.</returns>
    Task<ReportRenderResult> RenderAsync(IStructuredReport report, ReportRenderOptions renderOptions,
        CancellationToken cancellationToken = default);
}
