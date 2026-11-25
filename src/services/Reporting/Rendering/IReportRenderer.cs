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

    /// <summary>
    /// Renders a report asynchronously, e.g. to PDF, which takes a while. The default implementation calls
    /// <see cref="Render"/>.
    /// </summary>
    /// <param name="report">Report object</param>
    /// <param name="renderOptions">Report render options</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The render result.</returns>
    public Task<ReportRenderResult> RenderAsync(IReport report, ReportRenderOptions renderOptions,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Render(report, renderOptions));
    }
}