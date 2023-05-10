
namespace Serenity.Reporting;

/// <summary>
/// Excel exporter interface for data only reports. This is usually
/// implemented by EPPlus based exporter. The interface abstracts
/// that dependency.
/// </summary>
public interface IDataReportExcelRenderer
{
    /// <summary>
    /// Renders the specified report to Excel format.
    /// </summary>
    /// <param name="report">The report.</param>
    byte[] Render(IDataOnlyReport report);
}