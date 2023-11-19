namespace Serenity.ComponentModel;

/// <summary>
/// Determines type of aggregate function for a column to use
/// </summary>
/// <remarks>
/// Creates a new instance of SummaryTypeAttribute.
/// </remarks>
/// <param name="value">Aggregate function type</param>
public class SummaryTypeAttribute(SummaryType value) : Attribute
{

    /// <summary>
    /// Gets aggregate function type
    /// </summary>
    public SummaryType Value { get; private set; } = value;
}
