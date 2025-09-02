namespace Serenity;

/// <summary>
/// Enumeration of aggregate function types for a column
/// </summary>
public enum SummaryType
{
    /// <summary>
    /// Summary popup is disabled for this column
    /// </summary>
    Disabled = -1,
    /// <summary>
    /// No aggregate by default, but user might change
    /// </summary>
    None = 0,
    /// <summary>
    /// Use Sum aggregate by default
    /// </summary>
    Sum = 1,
    /// <summary>
    /// Use Avg aggregate by default
    /// </summary>
    Avg = 2,
    /// <summary>
    /// Use Min aggregate by default
    /// </summary>
    Min = 3,
    /// <summary>
    /// Use Max aggregate by default
    /// </summary>
    Max = 4
}
