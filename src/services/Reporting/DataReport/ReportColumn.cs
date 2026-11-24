namespace Serenity.Reporting;

/// <summary>
/// Information about a report column.
/// </summary>
public class ReportColumn
{
    /// <summary>
    /// Gets or sets the property name or field name of the column.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the title for the column.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the preferred width of the column.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Gets or sets the data type of values for the column.
    /// </summary>
    public Type? DataType { get; set; }

    /// <summary>
    /// Gets or sets the format string to use.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text should be wrapped.
    /// </summary>
    public bool WrapText { get; set; }

    /// <summary>
    /// Gets or sets the decorator to use for the column.
    /// </summary>
    public ICellDecorator? Decorator { get; set; }

    /// <summary>
    /// Gets or sets the kind of the values of the column, e.g. money, which decides how they are
    /// formatted when the column has no <see cref="Format"/>.
    /// </summary>
    public ReportValueKind Kind { get; set; }

    /// <summary>
    /// Gets or sets how the total of the column is computed.
    /// </summary>
    public ReportTotalKind Total { get; set; }

    /// <summary>
    /// Gets or sets the link of the values of the column, with the names of the properties of the row in braces, e.g.
    /// "~/Northwind/Customer#view/{CustomerID}". Only a report that is shown on a page uses it.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets the name of the property of the row that holds the tone of the value, i.e. "ok", "warn", "danger", "info"
    /// or "muted", which shows the value as a badge. Only a report that is shown on a page uses it.
    /// </summary>
    public string? Tone { get; set; }
}