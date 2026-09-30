namespace Serenity.Data;

/// <summary>
/// Delta options flags.
/// </summary>
[Flags]
public enum DeltaOptions
{
    /// <summary>
    /// The default options (strict: a new item id unknown in the old list throws).
    /// Pass <see cref="IgnoreInvalidNewId"/> explicitly to turn such items into inserts.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Ignore new item identifiers that are not present in the old list.
    /// </summary>
    IgnoreInvalidNewId = 1
}