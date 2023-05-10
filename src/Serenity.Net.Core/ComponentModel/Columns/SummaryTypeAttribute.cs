namespace Serenity.ComponentModel;

/// <summary>
/// Determines type of aggregate function for a column to use
/// </summary>
public class SummaryTypeAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of SummaryTypeAttribute.
    /// </summary>
    /// <param name="value">Aggregate function type</param>
    public SummaryTypeAttribute(SummaryType value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets aggregate function type
    /// </summary>
    public SummaryType Value { get; private set; }
}
