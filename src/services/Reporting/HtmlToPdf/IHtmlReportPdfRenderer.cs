
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

    /// <summary>
    /// Renders the specified report to PDF asynchronously. The default implementation calls <see cref="Render"/>.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">Report render options</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<byte[]> RenderAsync(IReport report, ReportRenderOptions renderOptions, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Render(report, renderOptions));
    }
}