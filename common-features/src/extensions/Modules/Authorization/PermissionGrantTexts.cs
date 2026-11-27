namespace Serenity.Extensions;

/// <summary>
/// The messages of <see cref="PermissionGrantValidator"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Validation.PermissionGrant.")]
public static class PermissionGrantTexts
{
    /// <summary>A permission that only a super admin can grant.</summary>
    public static readonly LocalText SuperAdminOnly = "Only a super admin can grant or revoke {0}.";

    /// <summary>A permission that only its holders can grant.</summary>
    public static readonly LocalText NotHeld = "You can't grant or revoke {0}, as you don't have it.";

    /// <summary>A permission that the application does not list, e.g. a private one.</summary>
    public static readonly LocalText NotListed = "{0} is not a permission that can be granted.";

    /// <summary>The account of a super admin.</summary>
    public static readonly LocalText UserSuperAdmin = "Only a super admin can change the account of {0}.";

    /// <summary>The account of a user with more rights.</summary>
    public static readonly LocalText UserNotHeld = "You can't change the account of {0}, who has permissions that you can't grant.";
}
