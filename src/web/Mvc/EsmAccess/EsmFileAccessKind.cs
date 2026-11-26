namespace Serenity.Web;

/// <summary>
/// The kind of access to a file that <see cref="EsmAccessPolicy"/> determines.
/// </summary>
public enum EsmFileAccessKind
{
    /// <summary>
    /// Not an ESM file the policy handles, e.g. outside an <c>esm</c> folder or in one without a
    /// manifest, which is served as usual.
    /// </summary>
    NotHandled,
    /// <summary>
    /// A file that is never served, e.g. the manifest itself.
    /// </summary>
    Hidden,
    /// <summary>
    /// A file that is served to everyone, including anonymous users.
    /// </summary>
    Public,
    /// <summary>
    /// A file that is served only to users with one of the permissions.
    /// </summary>
    Restricted
}
