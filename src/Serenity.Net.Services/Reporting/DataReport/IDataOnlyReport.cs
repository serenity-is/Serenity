namespace Serenity.Reporting;

/// <summary>
/// Interface for reports that only contain data, e.g. no design.
/// These are usually Excel/CSV etc. export type of reports.
/// </summary>
public interface IDataOnlyReport : IReport
{
    /// <summary>
    /// Gets the list of columns to export.
    /// </summary>
    List<ReportColumn> GetColumnList();
}