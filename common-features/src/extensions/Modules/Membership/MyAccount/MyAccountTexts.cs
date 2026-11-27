namespace Serenity.Extensions;

/// <summary>
/// The texts of the links of the my account page that the extensions give, see <see cref="PasswordMyAccountLinkProvider"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Forms.Membership.MyAccount.", Permission = SpecialPermissionKeys.LoggedIn)]
public static class MyAccountTexts
{
    public static readonly LocalText ChangePasswordDescription = "Change the password you sign in with.";
    public static readonly LocalText ChangePasswordRequired = "Change required";
    public static readonly LocalText PasswordTitle = "Password";
    public static readonly LocalText SetPasswordDescription = "Your account has no password. Set one, e.g. to confirm sensitive actions.";
}
