namespace Serenity.Reporting;

/// <summary>
/// Sets the options of the report column of a property, e.g. the kind of its values and its total, which
/// <see cref="ReportColumnConverter"/> copies to the <see cref="ReportColumn"/> it creates.
/// </summary>
/// <param name="kind">The kind of the values of the column.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ReportColumnOptionsAttribute(ReportValueKind kind = ReportValueKind.Default) : Attribute
{
    /// <summary>
    /// The kind of the values of the column.
    /// </summary>
    public ReportValueKind Kind { get; } = kind;

    /// <summary>
    /// How the total of the column is computed.
    /// </summary>
    public ReportTotalKind Total { get; set; }

    /// <summary>
    /// The link of the values of the column, with the names of the properties of the row in braces, e.g.
    /// "~/Northwind/Customer#view/{CustomerID}".
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// The name of the property of the row that holds the tone of the value, i.e. "ok", "warn", "danger", "info" or
    /// "muted", which shows the value as a badge.
    /// </summary>
    public string? Tone { get; set; }
}
