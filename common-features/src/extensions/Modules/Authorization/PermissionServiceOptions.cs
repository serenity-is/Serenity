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
    public const string DefaultSuperAdmins = "admin";

    /// <summary>
    /// The usernames of the super admins, who have every permission and are the only ones who can impersonate other
    /// users, compared ignoring case, e.g. "admin, ops", separated by commas, semicolons or white space (see
    /// <see cref="SettingsList"/>). When not set, <see cref="DefaultSuperAdmins"/>; an empty text means no super admin.
    /// It is a text rather than an array, as a settings file or an environment variable replaces a text as a whole,
    /// where the items of an array would be merged by their index.
    /// </summary>
    public string? SuperAdmins { get; set; }

    /// <summary>
    /// Checks whether a user is a super admin.
    /// </summary>
    /// <param name="username">The username.</param>
    public bool IsSuperAdmin(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return false;

        foreach (var superAdmin in SettingsList.Split(SuperAdmins ?? DefaultSuperAdmins))
            if (string.Equals(superAdmin, username, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }
}
