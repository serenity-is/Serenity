namespace Serene.Membership;

[NestedLocalTexts(Prefix = "Validation.")]
public static class MembershipValidationTexts
{
    public static readonly LocalText AuthenticationError = "Invalid username or password!";
    public static readonly LocalText CurrentPasswordMismatch = "Your current password is not valid!";
    public static readonly LocalText MinRequiredPasswordLength = "Entered password doesn't have enough characters (min {0})!";
    public static readonly LocalText PasswordConfirmMismatch = "The passwords entered doesn't match!";
    public static readonly LocalText InvalidResetToken = "Your token to reset your password is invalid or has expired!";
    public static readonly LocalText InvalidActivateToken = "Your token to activate your account is invalid or has expired!";
    public static readonly LocalText EmailInUse = "Another user with this email exists!";
    public static readonly LocalText EmailConfirm = "Emails entered doesn't match!";
}