namespace Serenity.Extensions;

/// <summary>
/// The texts of the emails that tell the users their account changed, see <see cref="AccountEventEmailHandler"/>. The
/// placeholders are {0} for the old value and {1} for the new one, or {0} for the two-factor method.
/// </summary>
[NestedLocalTexts(Prefix = "Emails.AccountEvent.")]
public static class AccountEventEmailTexts
{
    public static readonly LocalText PasswordChangedSubject = "Your password was changed";
    public static readonly LocalText PasswordChanged = "The password of your account was changed.";
    public static readonly LocalText EmailChangedSubject = "Your email address was changed";
    public static readonly LocalText EmailChanged = "The email address of your account was changed from {0} to {1}.";
    public static readonly LocalText UsernameChangedSubject = "Your username was changed";
    public static readonly LocalText UsernameChanged = "The username of your account was changed from {0} to {1}. Use the new one to sign in.";
    public static readonly LocalText TwoFactorEnabledSubject = "A two-factor authentication method was added";
    public static readonly LocalText TwoFactorEnabled = "{0} was set up for two-factor authentication on your account.";
    public static readonly LocalText TwoFactorDisabledSubject = "A two-factor authentication method was removed";
    public static readonly LocalText TwoFactorDisabled = "{0} was removed from the two-factor authentication of your account.";
    public static readonly LocalText TwoFactorResetSubject = "Your two-factor authentication was reset";
    public static readonly LocalText TwoFactorReset = "The two-factor authentication of your account was reset, so you can set it up again the next time you sign in.";

    public static readonly LocalText Greeting = "Hello {0},";
    public static readonly LocalText BySelf = "This was done from your account.";
    public static readonly LocalText ByOther = "This was done by {0}.";
    public static readonly LocalText When = "When: {0} (UTC)";
    public static readonly LocalText Where = "IP address: {0}";
    public static readonly LocalText NotYou = "If you didn't expect this, contact your administrator right away.";
    public static readonly LocalText Signature = "The {0} team";
}
