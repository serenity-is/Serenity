namespace Serenity.Data;

/// <summary>
/// Delta options flags.
/// </summary>
[Flags]
public enum DeltaOptions
{
    /// <summary>
    /// The default options.
    /// </summary>
    Default = IgnoreInvalidNewId,
    /// <summary>
    /// Ignore new item identifiers that are not present in the old list.
    /// </summary>
    IgnoreInvalidNewId = 1
}