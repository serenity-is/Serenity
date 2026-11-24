namespace Serenity.Reporting;

/// <summary>
/// A table of a <see cref="StructuredReportResult"/>.
/// </summary>
public class StructuredTable
{
    /// <summary>
    /// The key of the table, which should be unique in its result.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The title of the table, e.g. the name of its sheet in an Excel export.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The columns of the table. <see cref="ReportColumnConverter.ObjectTypeToList"/> can create them from the type of its rows.
    /// </summary>
    public List<ReportColumn> Columns { get; set; } = [];

    /// <summary>
    /// The rows of the table, e.g. objects, entity rows or dictionaries, which have a value for the name of each column.
    /// </summary>
    public System.Collections.IList Rows { get; set; } = Array.Empty<object>();

    /// <summary>
    /// The totals of the columns by their names, which are used instead of the ones <see cref="ReportColumn.Total"/> computes,
    /// e.g. for a column whose total is a ratio of the totals of two others, like an average order value.
    /// </summary>
    public Dictionary<string, object?>? Totals { get; set; }
}
