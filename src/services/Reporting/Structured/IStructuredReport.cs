namespace Serenity.Reporting;

/// <summary>
/// A report that returns a structured result, i.e. one or more tables with an optional set of KPIs and a chart,
/// instead of a design to render or a URL to open. The same result can be shown on a page, exported to Excel with a
/// sheet per table, or to CSV with its main table, so the report does not need a separate design for each.
/// </summary>
public interface IStructuredReport : IReport
{
    /// <summary>
    /// Runs the report with its current parameters and returns its result.
    /// </summary>
    StructuredReportResult GetResult();

    /// <inheritdoc/>
    object? IReport.GetData() => GetResult();
}
