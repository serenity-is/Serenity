namespace Serenity.Reporting;

/// <summary>
/// Cell decorator interface mainly used for Excel export
/// styling.
/// </summary>
public interface ICellDecorator
{
    /// <summary>
    /// The input item
    /// </summary>
    object Item { get; set; }

    /// <summary>
    /// The column name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Value of the current cell. If desired, it can be 
    /// modified by the decorator.
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// Assign to set background color of the current cell
    /// </summary>
    string Background { get; set; }

    /// <summary>
    /// Assign to set text color of the current cell
    /// </summary>
    string Foreground { get; set; }

    /// <summary>
    /// Assign to set display format of the current cell.
    /// The format should be in Excel formatting style.
    /// </summary>
    string Format { get; set; }

    /// <summary>
    /// The decorator should apply its formatting 
    /// in this method. It is called by the exporter.
    /// </summary>
    void Decorate();
}