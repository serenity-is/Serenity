
namespace Serenity.Data;

/// <summary>
/// Sets default read permission for fields of a row which doesn't have a InsertPermission
/// themselves.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class FieldInsertPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldInsertPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public FieldInsertPermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldInsertPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public FieldInsertPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldInsertPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public FieldInsertPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}