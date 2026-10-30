namespace Serenity.Testing;

public class NullUserPasswordValidator : IUserPasswordValidator
{
    public PasswordValidationResult Validate(ref string username, string password)
    {
        return PasswordValidationResult.Invalid;
    }
}