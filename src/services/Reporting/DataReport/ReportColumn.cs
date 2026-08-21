namespace Serenity.Reporting;

/// <summary>
/// Information about a report column.
/// </summary>
public class ReportColumn
{
    /// <summary>
    /// Gets or sets the property name or field name of the column.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the title for the column.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the preferred width of the column.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Gets or sets the data type of values for the column.
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// Gets or sets the format string to use.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the text should be wrapped.
    /// </summary>
    public bool WrapText { get; set; }

    /// <summary>
    /// Gets or sets the decorator to use for the column.
    /// </summary>
    public ICellDecorator Decorator { get; set; }
}