namespace Serenity.ComponentModel;

/// <summary>
/// Determines the type of aggregate function to use for a column.
/// </summary>
/// <remarks>
/// Creates a new instance of SummaryTypeAttribute.
/// </remarks>
/// <param name="value">Aggregate function type.</param>
public class SummaryTypeAttribute(SummaryType value) : Attribute
{

    /// <summary>
    /// Gets the aggregate function type.
    /// </summary>
    public SummaryType Value { get; private set; } = value;
}
