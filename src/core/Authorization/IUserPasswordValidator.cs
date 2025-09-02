namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to validate a user password
/// </summary>
public interface IUserPasswordValidator
{
    /// <summary>
    /// Validates a user password
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns><see cref="PasswordValidationResult.Valid"/> if given username and password is true</returns>
    PasswordValidationResult Validate(ref string username, string password);
}