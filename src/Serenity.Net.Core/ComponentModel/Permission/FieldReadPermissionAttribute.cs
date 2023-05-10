
namespace Serenity.Data;

/// <summary>
/// Sets default read permission for fields of a row which doesn't have a ReadPermission
/// themselves.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class FieldReadPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReadPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public FieldReadPermissionAttribute(object permission)
        : base(permission)
    {
        ApplyToLookups = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReadPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public FieldReadPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
        ApplyToLookups = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReadPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public FieldReadPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
        ApplyToLookups = true;
    }

    /// <summary>
    /// Determines whether this permission is also applied to row fields with
    /// [LookupInclude] attribute, ID and Name fields. Defaults to true.
    /// </summary>
    public bool ApplyToLookups { get; set; }
}