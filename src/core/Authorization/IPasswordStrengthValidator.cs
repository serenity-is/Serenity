namespace Serenity.Abstractions;

/// <summary>
/// Validates that a password meets the configured strength requirements.
/// </summary>
public interface IPasswordStrengthValidator
{
    /// <summary>
    /// Validates the strength of the specified password.
    /// </summary>
    /// <param name="password">The plain-text password to validate.</param>
    /// <exception cref="ValidationError">Thrown when the password does not satisfy the strength rules.</exception>
    void Validate(string password);
}