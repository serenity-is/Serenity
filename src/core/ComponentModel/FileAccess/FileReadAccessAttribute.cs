namespace Serenity.Data;

/// <summary>
/// Base attribute to specify file read access control on a property.
/// </summary>

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileReadAccessAttribute : Attribute
{
    /// <summary>
    /// Permission to check for file read access. Use "*" for public/unrestricted,
    /// "?" for any logged in user, or a specific permission key.
    /// If using LogicOperatorPermissionService, permission can also be a logical expression
    /// like A|B&amp;!C.
    /// </summary>
    public string? Permission { get; protected internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether bypass is allowed for this operation.
    /// The Bypass permission is usually given to administrators and set in FileReadAccessSettings.
    /// Default is true, meaning that users with the bypass permission are allowed to read the file.
    /// </summary>
    public bool? AllowBypass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to skip entity-level access control
    /// and use only permission-based access. If this attribute specifies a permission,
    /// that permission is used. Otherwise, the default permission is used.
    /// </summary>
    public bool? PermissionOnly { get; set; }
}