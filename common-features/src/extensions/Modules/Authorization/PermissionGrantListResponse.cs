namespace Serenity.Extensions;

/// <summary>
/// The permissions that the current user can't grant or revoke, so a page can show them as read-only.
/// </summary>
public class PermissionGrantListResponse : ServiceResponse
{
    /// <summary>
    /// Whether the current user is a super admin, who can grant anything.
    /// </summary>
    public bool IsSuperAdmin { get; set; }

    /// <summary>
    /// The permission keys that the current user can't grant or revoke, with the reason: "SuperAdminOnly" or "NotHeld",
    /// see <see cref="PermissionGrantDenial"/>.
    /// </summary>
    public Dictionary<string, string>? Denied { get; set; }
}
