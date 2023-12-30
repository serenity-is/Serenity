namespace Serenity.Abstractions;

/// <summary>
/// Abstraction to validate the password rules
/// </summary>
public interface IPasswordRuleValidator
{

    /// <summary>
    /// Validates password rules
    /// </summary>
    /// <param name="password"></param>
    /// <exception cref="ValidationError">Throws validation error if password does not match the rules</exception>
    string ValidatePasswordRules(string password);
}