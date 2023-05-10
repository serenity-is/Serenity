namespace Serenity.Reporting;

/// <summary>
/// Information about a report column
/// </summary>
public class ReportColumn
{
    /// <summary>
    /// The property name or field name of the column
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Title for the column.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Preferred with of the column.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// The data type of values for the column.
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// The format string to use
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Should text be wrapped
    /// </summary>
    public bool WrapText { get; set; }

    /// <summary>
    /// Decorator to use for the column
    /// </summary>
    public ICellDecorator Decorator { get; set; }
}