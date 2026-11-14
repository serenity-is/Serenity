namespace Serenity;

/// <summary>
/// Enumeration of aggregate function types for a column
/// </summary>
public enum SummaryType
{
    /// <summary>
    /// Summary popup is disabled for this column
    /// </summary>
    [DescriptionText("Disabled")]
    Disabled = -1,
    /// <summary>
    /// No aggregate by default, but the user might change it.
    /// </summary>
    [DescriptionText("None")]
    None = 0,
    /// <summary>
    /// Use Sum aggregate by default
    /// </summary>
    [DescriptionText("Sum")]
    Sum = 1,
    /// <summary>
    /// Use Avg aggregate by default
    /// </summary>
    [DescriptionText("Avg")]
    Avg = 2,
    /// <summary>
    /// Use Min aggregate by default
    /// </summary>
    [DescriptionText("Min")]
    Min = 3,
    /// <summary>
    /// Use Max aggregate by default
    /// </summary>
    [DescriptionText("Max")]
    Max = 4
}
