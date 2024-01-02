namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to validate the password rules
/// </summary>
public interface IPasswordRuleValidator
{

    /// <summary>
    /// Validates password strength rules
    /// </summary>
    /// <param name="password"></param>
    /// <exception cref="ValidationError">Throws validation error if password does not match the strength rules</exception>
    string ValidatePasswordStrengthRules(string password);
}