namespace Serenity.Web;

/// <summary>
/// The access to a file that <see cref="EsmAccessPolicy"/> determines.
/// </summary>
public class EsmFileAccess
{
    private EsmFileAccess(EsmFileAccessKind kind, string[] permissions)
    {
        Kind = kind;
        Permissions = permissions;
    }

    /// <summary>
    /// The kind of access.
    /// </summary>
    public EsmFileAccessKind Kind { get; }

    /// <summary>
    /// For <see cref="EsmFileAccessKind.Restricted"/>, the permissions of the entry points that reach the file.
    /// The file is served to users with any of them.
    /// </summary>
    public string[] Permissions { get; }

    /// <summary>
    /// Not a file the policy handles.
    /// </summary>
    public static readonly EsmFileAccess NotHandled = new(EsmFileAccessKind.NotHandled, []);

    /// <summary>
    /// A file that is never served.
    /// </summary>
    public static readonly EsmFileAccess Hidden = new(EsmFileAccessKind.Hidden, []);

    /// <summary>
    /// A file that is served to everyone.
    /// </summary>
    public static readonly EsmFileAccess Public = new(EsmFileAccessKind.Public, []);

    /// <summary>
    /// A file that is served only to users with one of the permissions.
    /// </summary>
    /// <param name="permissions">The permissions</param>
    public static EsmFileAccess Restricted(string[] permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        return new(EsmFileAccessKind.Restricted, permissions);
    }
}
