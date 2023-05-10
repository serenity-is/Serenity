namespace Serenity.Data;

/// <summary>
/// Delta options flags
/// </summary>
[Flags]
public enum DeltaOptions
{
    /// <summary>
    /// The default
    /// </summary>
    Default = IgnoreInvalidNewId,
    /// <summary>
    /// The ignore invalid new identifier
    /// </summary>
    IgnoreInvalidNewId = 1
}