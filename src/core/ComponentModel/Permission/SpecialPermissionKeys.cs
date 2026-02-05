namespace Serenity;

/// <summary>
/// Contains special permission key constants.
/// </summary>
public static class SpecialPermissionKeys
{
    /// <summary>
    /// Wildcard ("*") permission key for public/unrestricted access.
    /// </summary>
    public const string Public = "*";

    /// <summary>
    /// Question mark ("?") permission key for any logged in user access.
    /// </summary>
    public const string LoggedIn = "?";

    /// <summary>
    /// Deny permission key ("DENY") for denying all access, even to super admins.
    /// </summary>
    public const string Deny = "DENY";
}