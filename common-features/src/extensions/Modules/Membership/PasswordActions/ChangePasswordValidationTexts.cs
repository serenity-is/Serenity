namespace Serenity.Extensions;

/// <summary>
/// Local text keys for change password validation.
/// </summary>
[NestedLocalTexts(Prefix = "Validation.")]
public static class ChangePasswordValidationTexts
{
    public static readonly LocalText InvalidResetToken = "Your token to reset your password is invalid or has expired!";
    public static readonly LocalText PasswordConfirmMismatch = "The passwords entered doesn't match!";
}