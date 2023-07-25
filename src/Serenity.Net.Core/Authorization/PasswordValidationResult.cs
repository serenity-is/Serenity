namespace Serenity.ComponentModel;

/// <summary>
/// Password validation result
/// </summary>
public enum PasswordValidationResult
{
    /// <summary>
    /// Username is empty
    /// </summary>
    EmptyUsername,
    /// <summary>
    /// Password is empty
    /// </summary>
    EmptyPassword,
    /// <summary>
    /// User is not active
    /// </summary>
    InactiveUser,
    /// <summary>
    /// User source is not found
    /// </summary>
    UnknownSource,
    /// <summary>
    /// To many retries
    /// </summary>
    Throttle,
    /// <summary>
    /// Directory error
    /// </summary>
    DirectoryError,
    /// <summary>
    /// Invalid
    /// </summary>
    Invalid,
    /// <summary>
    /// Valid
    /// </summary>
    Valid
}