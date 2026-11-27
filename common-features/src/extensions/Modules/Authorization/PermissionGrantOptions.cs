namespace Serenity.Extensions;

/// <summary>
/// Who can grant or revoke which permissions, read from the "PermissionGrants" section of the settings, as a list of
/// permission key patterns (see <see cref="PermissionPattern"/>) for each <see cref="PermissionGrantTier"/>, separated by
/// commas, semicolons or white space (see <see cref="SettingsList"/>), e.g.
/// <code>
/// "PermissionGrants": {
///   "Grantable": "*",
///   "HolderOnly": "Administration:*",
///   "SuperAdminOnly": "Administration:Security, Administration:Credentials"
/// }
/// </code>
/// A key gets the most restrictive tier with a pattern that matches it, so a broad "Grantable" never loosens a stricter
/// one, and a key that matches no pattern is <see cref="PermissionGrantTier.HolderOnly"/>. Without the section, everything
/// is grantable, as before. The lists are texts rather than arrays, as a settings file or an environment variable replaces
/// a text as a whole, where the items of an array would be merged by their index.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class PermissionGrantOptions
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "PermissionGrants";

    /// <summary>
    /// The grantable patterns when <see cref="Grantable"/> is not set: everything.
    /// </summary>
    public const string DefaultGrantable = "*";

    /// <summary>
    /// The patterns of the permissions that any administrator who can manage permissions can grant, also one who does not
    /// have them. When not set, <see cref="DefaultGrantable"/>; an empty text means none.
    /// </summary>
    public string? Grantable { get; set; }

    /// <summary>
    /// The patterns of the permissions that only an administrator who has them can grant.
    /// </summary>
    public string? HolderOnly { get; set; }

    /// <summary>
    /// The patterns of the permissions that only a super admin can grant.
    /// </summary>
    public string? SuperAdminOnly { get; set; }

    /// <summary>
    /// Gets who can grant a permission: the most restrictive tier with a pattern that matches it, else
    /// <see cref="PermissionGrantTier.HolderOnly"/>.
    /// </summary>
    /// <param name="permissionKey">The permission key.</param>
    /// <returns>The tier.</returns>
    public PermissionGrantTier GetTier(string permissionKey)
    {
        if (IsMatch(SuperAdminOnly, permissionKey))
            return PermissionGrantTier.SuperAdminOnly;

        if (IsMatch(HolderOnly, permissionKey))
            return PermissionGrantTier.HolderOnly;

        if (IsMatch(Grantable ?? DefaultGrantable, permissionKey))
            return PermissionGrantTier.Grantable;

        return PermissionGrantTier.HolderOnly;
    }

    private static bool IsMatch(string? patterns, string permissionKey)
    {
        foreach (var pattern in SettingsList.Split(patterns))
            if (PermissionPattern.IsMatch(pattern, permissionKey))
                return true;

        return false;
    }
}
