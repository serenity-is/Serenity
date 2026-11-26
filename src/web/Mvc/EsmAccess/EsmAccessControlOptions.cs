namespace Serenity.Web;

/// <summary>
/// Options for the ESM access control (see <c>UseEsmAccessControl</c>), which serves the ES modules
/// and chunks under an <c>esm</c> folder (e.g. <c>/esm/</c> or <c>/Serenity.Extensions/esm/</c>) only to
/// users who may load an entry point (e.g. a page module) that needs them, using the
/// <c>build.esm-manifest</c> that tsbuild writes next to them.
/// </summary>
/// <remarks>
/// <para>Each file is served if the user has the permission of at least one entry point that reaches
/// it via its imports. The permission of an entry point is, in order:</para>
/// <list type="number">
/// <item>the <see cref="LayoutScriptFileAttribute.Permission"/> if it is a layout module,</item>
/// <item>"*" (public) if it matches <see cref="AnonymousPaths"/>,</item>
/// <item>the permission of the longest matching pattern in <see cref="Permissions"/>, if any,</item>
/// <item><see cref="DefaultPermission"/>.</item>
/// </list>
/// <para>So unless <see cref="Permissions"/> is set, anonymous users only get the anonymous and layout
/// module files, while logged-in users get all of them. A file that is not in the manifest is treated
/// as an entry point of its own. Files of an <c>esm</c> folder without a manifest are served as before.</para>
/// <para>Patterns are semicolon separated globs (<c>*</c>, <c>**</c>, <c>?</c>), case-insensitive, relative
/// to the <c>esm</c> folder (e.g. <c>Modules/Membership/**</c>), or the full request path if they start
/// with a slash (e.g. <c>/Serenity.Extensions/esm/Modules/Membership/**</c>).</para>
/// </remarks>
[DefaultSectionKey(SectionKey)]
public class EsmAccessControlOptions
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "EsmAccessControl";

    /// <summary>
    /// Whether the ESM access control is enabled. Default is false.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Semicolon separated patterns of the entry points (or other files) that anonymous users can load,
    /// e.g. the login page. The files that these entry points reach are public too.
    /// </summary>
    public string? AnonymousPaths { get; set; } = "Modules/Common/ScriptInit.js;Modules/Membership/**;bundles/**";

    /// <summary>
    /// Optional patterns of entry points mapped to the permission (or permission expression) required to
    /// load them, e.g. <c>{ "Modules/Administration/**": "Administration:General" }</c>. When a path matches
    /// more than one pattern, the longest pattern wins. Empty by default, in which case entry points that
    /// are not anonymous use <see cref="DefaultPermission"/>.
    /// Note that a page may load modules of another page, e.g. to open a dialog of another module, which
    /// fails for users without the permission of that module.
    /// </summary>
    public Dictionary<string, string> Permissions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The permission of entry points that do not match any other rule. Default is "?", e.g. logged-in users.
    /// Like other permissions, an empty value is invalid and denies access.
    /// </summary>
    public string DefaultPermission { get; set; } = SpecialPermissionKeys.LoggedIn;
}
