
namespace Serenity.Data;

/// <summary>
/// Sets update permission for the row.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class UpdatePermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public UpdatePermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public UpdatePermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public UpdatePermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}