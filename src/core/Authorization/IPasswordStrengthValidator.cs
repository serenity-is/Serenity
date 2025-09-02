namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to validate the password strength
/// </summary>
public interface IPasswordStrengthValidator
{

    /// <summary>
    /// Validates password strength
    /// </summary>
    /// <param name="password"></param>
    /// <exception cref="ValidationError">Throws validation error if password 
    /// does not match the expected password strength rules</exception>
    void Validate(string password);
}