namespace Serenity.Extensions;

/// <summary>
/// The texts of what a user must do before using the application, see <see cref="IAccountSetupRequirement"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Forms.Membership.AccountSetup.")]
public static class AccountSetupTexts
{
    public static readonly LocalText PasswordChangeRequired = "Change your password to continue.";
    public static readonly LocalText PasswordChangeRequiredTitle = "Change your password";
    public static readonly LocalText PasswordChangeRequiredHint = "Your password was set by an administrator. Choose a new one to continue to the application.";
}
