namespace Serenity.Extensions;

/// <summary>
/// Why the current user can't grant a permission, or change the account of a user, see <see cref="IPermissionGrantValidator"/>.
/// </summary>
public enum PermissionGrantDenial
{
    /// <summary>
    /// Only a super admin can, e.g. a <see cref="PermissionGrantTier.SuperAdminOnly"/> permission, or the account of a
    /// super admin.
    /// </summary>
    SuperAdminOnly = 1,

    /// <summary>
    /// The current user does not have the permission, which is <see cref="PermissionGrantTier.HolderOnly"/>, or the user
    /// whose account it is has such a permission.
    /// </summary>
    NotHeld = 2
}
