namespace Serenity.Extensions;

/// <summary>
/// Who can grant or revoke a permission, from the least to the most restrictive, see <see cref="PermissionGrantOptions"/>.
/// </summary>
public enum PermissionGrantTier
{
    /// <summary>
    /// Any administrator who can manage permissions, also one who does not have it.
    /// </summary>
    Grantable = 0,

    /// <summary>
    /// Only an administrator who has it, e.g. a sensitive one that should not be handed out by someone without it.
    /// </summary>
    HolderOnly = 1,

    /// <summary>
    /// Only a super admin, e.g. the permission of managing users, so that its holders can't make others like them.
    /// </summary>
    SuperAdminOnly = 2
}
