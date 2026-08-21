namespace Serenity.Abstractions;

/// <summary>
/// Validates a username and password combination.
/// </summary>
public interface IUserPasswordValidator
{
    /// <summary>
    /// Validates the specified username and password.
    /// </summary>
    /// <param name="username">The username to validate. The implementation may normalize the value in place.</param>
    /// <param name="password">The plain-text password to validate.</param>
    /// <returns>
    /// <see cref="PasswordValidationResult.Valid"/> when the credentials are valid; otherwise a value
    /// indicating the reason for failure.
    /// </returns>
    PasswordValidationResult Validate(ref string username, string password);
}