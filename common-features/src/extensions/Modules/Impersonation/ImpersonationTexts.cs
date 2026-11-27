namespace Serenity.Extensions;

/// <summary>
/// The text of an action that is not available while the user is impersonated, see
/// <see cref="ImpersonationExtensions.ValidateNotImpersonating"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Forms.Membership.Impersonation.", Permission = SpecialPermissionKeys.LoggedIn)]
public static class ImpersonationTexts
{
    public static readonly LocalText Restricted = "This isn't available while you are signed in as another user. Return to your own account first.";
}
