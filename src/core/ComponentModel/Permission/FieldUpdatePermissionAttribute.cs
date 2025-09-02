
namespace Serenity.Data;

/// <summary>
/// Sets default read permission for fields of a row which doesn't have a UpdatePermission
/// themselves.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class FieldUpdatePermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldUpdatePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public FieldUpdatePermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldUpdatePermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public FieldUpdatePermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldUpdatePermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public FieldUpdatePermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}