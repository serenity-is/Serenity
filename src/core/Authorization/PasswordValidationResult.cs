namespace Serenity.ComponentModel;

/// <summary>
/// Represents the result of a password validation attempt.
/// </summary>
public enum PasswordValidationResult
{
    /// <summary>
    /// The username was empty.
    /// </summary>
    EmptyUsername,
    /// <summary>
    /// The password was empty.
    /// </summary>
    EmptyPassword,
    /// <summary>
    /// The user account is not active.
    /// </summary>
    InactiveUser,
    /// <summary>
    /// The user source could not be found.
    /// </summary>
    UnknownSource,
    /// <summary>
    /// The request was throttled due to too many attempts.
    /// </summary>
    Throttle,
    /// <summary>
    /// A directory service error occurred.
    /// </summary>
    DirectoryError,
    /// <summary>
    /// The credentials are invalid.
    /// </summary>
    Invalid,
    /// <summary>
    /// The credentials are valid.
    /// </summary>
    Valid
}