namespace Serenity.Extensions;

[NestedLocalTexts(Prefix = "Validation.")]
public static class ChangePasswordValidationTexts
{
    public static readonly LocalText InvalidResetToken = "Your token to reset your password is invalid or has expired!";
    public static readonly LocalText PasswordConfirmMismatch = "The passwords entered doesn't match!";
}