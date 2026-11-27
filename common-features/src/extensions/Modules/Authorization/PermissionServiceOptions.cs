namespace Serenity.Extensions;

/// <summary>
/// Options of the permission service of the application, see <see cref="BasePermissionService"/>, read from the
/// "PermissionService" section of the settings.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class PermissionServiceOptions
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "PermissionService";

    /// <summary>
    /// The super admins when <see cref="SuperAdmins"/> is not set: the "admin" user of the templates.
    /// </summary>
    public static IReadOnlyList<string> DefaultSuperAdmins { get; } = ["admin"];

    /// <summary>
    /// The usernames of the super admins, who have every permission and are the only ones who can impersonate other
    /// users, compared ignoring case. When not set, <see cref="DefaultSuperAdmins"/>; an empty list means no super
    /// admin. It is an array that is null by default, as the settings would add to a list with a default value instead
    /// of replacing it.
    /// </summary>
    public string[]? SuperAdmins { get; set; }

    /// <summary>
    /// Checks whether a user is a super admin.
    /// </summary>
    /// <param name="username">The username.</param>
    public bool IsSuperAdmin(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        foreach (var superAdmin in SuperAdmins ?? DefaultSuperAdmins)
            if (string.Equals(superAdmin?.Trim(), username, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }
}
