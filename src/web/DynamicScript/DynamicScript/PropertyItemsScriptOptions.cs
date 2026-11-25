namespace Serenity.Web;

/// <summary>
/// Options for form and columns scripts permissions.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class PropertyItemsScriptOptions
{
    /// <summary>
    /// Default section key for these options
    /// </summary>
    public const string SectionKey = "PropertyItemsScripts";

    /// <summary>
    /// When true (default), form and columns scripts without an explicit Permission in
    /// their attribute are public, as in previous versions. When false, they require
    /// the ReadPermission of the row type in their BasedOnRow attribute (DENY if the row
    /// has none, like lookup scripts), or the <see cref="FallbackPermission"/> if they
    /// have no BasedOnRow attribute.
    /// An explicit Permission in FormScript / ColumnsScript attribute is always used.
    /// </summary>
    public bool PublicByDefault { get; set; } = true;

    /// <summary>
    /// Permission used for form and columns scripts without a BasedOnRow attribute, when
    /// <see cref="PublicByDefault"/> is false. Defaults to <see cref="SpecialPermissionKeys.Deny"/>.
    /// It does not apply to scripts based on a row without a ReadPermission, which are always denied.
    /// </summary>
    public string? FallbackPermission { get; set; } = SpecialPermissionKeys.Deny;

    /// <summary>
    /// Determines the permission for a form or columns type.
    /// </summary>
    /// <param name="type">Form or columns type</param>
    /// <param name="explicitPermission">Permission specified in its attribute</param>
    /// <returns>The permission, or null if the script should be public</returns>
    public string? GetPermissionFor(Type type, string? explicitPermission)
    {
        if (!string.IsNullOrEmpty(explicitPermission))
            return explicitPermission;

        if (PublicByDefault)
            return null;

        return PropertyItemsScript.GetDefaultPermission(type) ??
            (string.IsNullOrEmpty(FallbackPermission) ? SpecialPermissionKeys.Deny : FallbackPermission);
    }
}
