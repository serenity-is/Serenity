namespace Serenity.Web;

/// <summary>
/// Options for filtering the local texts sent to the client by the permissions of the current user.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class LocalTextPermissionOptions
{
    /// <summary>
    /// Default section key for these options
    /// </summary>
    public const string SectionKey = "LocalTextPermissions";

    /// <summary>
    /// When true, local text packages sent to the client only contain the texts the current user
    /// may see. The texts of a row ("Db.{LocalTextPrefix}.") require its ReadPermission (denied when
    /// it has none), the texts of a form or columns type require the permission of its script,
    /// the texts of an enum require its RequiredPermission (or ReadPermission) attribute, or when it
    /// has none, are only sent when a form or columns script the user can access uses that enum, the
    /// texts of a NestedLocalTexts class with a Permission require that permission, and permission key
    /// texts require <see cref="PermissionTextsPermission"/>. Texts used by the forms and columns the
    /// user can access are always sent, as are texts that don't belong to any of these, like
    /// "Controls.", "Dialogs.", "Pages.", "Validation." or "Site." texts. Defaults to false.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Permission required for the texts of permission keys ("Permission."), which
    /// are only needed by permission editors. Defaults to <see cref="SpecialPermissionKeys.LoggedIn"/>.
    /// </summary>
    public string? PermissionTextsPermission { get; set; } = SpecialPermissionKeys.LoggedIn;

    /// <summary>
    /// Local text key prefixes that are always sent, regardless of permissions,
    /// e.g. ["Db.Northwind."] to opt out a group of texts from filtering.
    /// </summary>
    public string[]? AlwaysInclude { get; set; }

    /// <summary>
    /// Local text key prefixes and the permission they require, e.g. { "Site.MyModule.": "MyModule:General" },
    /// for texts that can't have a NestedLocalTexts class with a Permission, like ones that only exist in
    /// JSON files. Texts used by the forms and columns the user can access are still sent.
    /// </summary>
    public Dictionary<string, string>? Restrict { get; set; }

    /// <summary>
    /// Users with this permission get all texts, as if filtering was disabled. For example "?" only
    /// filters the texts sent to anonymous users, which is simpler, and still hides the application
    /// structure from visitors that are not logged in. Combine permissions with "|" and "&amp;" when
    /// needed. Null by default, e.g. all users are filtered.
    /// </summary>
    public string? UnfilteredPermission { get; set; }
}
