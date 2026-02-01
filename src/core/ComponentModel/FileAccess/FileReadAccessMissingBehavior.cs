namespace Serenity;

/// <summary>
/// Specifies the behavior to use when a file read operation encounters missing access permissions.
/// </summary>
/// <remarks>Use this enumeration to control how the system responds when a file cannot be read due to
/// insufficient permissions. The selected value determines whether the operation is allowed, denied, or handled
/// according to default permission settings.</remarks>
public enum FileReadAccessMissingBehavior
{
    /// <summary>
    /// Specifies that the associated action or permission is allowed.
    /// </summary>
    Allow = 1,
    /// <summary>
    /// Specifies that access is explicitly denied.
    /// </summary>
    Deny = 2,
    /// <summary>
    /// Specifies the default permission used when no explicit permission is set.
    /// </summary>
    DefaultPermission = 3
}