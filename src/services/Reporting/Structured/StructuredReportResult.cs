namespace Serenity.Reporting;

/// <summary>
/// The result of an <see cref="IStructuredReport"/>.
/// </summary>
public class StructuredReportResult
{
    /// <summary>
    /// The tables of the result. The first one is the main table, e.g. the one a CSV export contains, and the others
    /// are details, which an Excel export puts on sheets of their own.
    /// </summary>
    public List<StructuredTable> Tables { get; set; } = [];

    /// <summary>
    /// The key figures shown above the tables, if any.
    /// </summary>
    public List<StructuredKpi>? Kpis { get; set; }

    /// <summary>
    /// The chart shown above the tables, if any.
    /// </summary>
    public StructuredChart? Chart { get; set; }

    /// <summary>
    /// The text to show when the main table has no rows, instead of a generic one.
    /// </summary>
    public string? EmptyText { get; set; }
}
