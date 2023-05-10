
namespace Serenity.Data;

/// <summary>
/// Sets delete permission for the row.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class DeletePermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public DeletePermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public DeletePermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public DeletePermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}