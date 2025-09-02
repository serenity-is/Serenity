namespace Serenity;

/// <summary>
/// Contains capture logging operation types
/// </summary>
public enum CaptureOperationType
{
    /// <summary>
    /// The before record, only available for updates
    /// </summary>
    Before = 0,
    /// <summary>
    /// Delete
    /// </summary>
    Delete = 1,
    /// <summary>
    /// Insert
    /// </summary>
    Insert = 2,
    /// <summary>
    /// Update (update records have both Before and Update records)
    /// </summary>
    Update = 3
}