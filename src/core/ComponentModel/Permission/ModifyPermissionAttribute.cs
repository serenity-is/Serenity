
namespace Serenity.Data;

/// <summary>
/// Sets modify permission for the row.
/// Modify permission is used for Insert/Update/Delete if they are
/// not explicitly specified.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class ModifyPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public ModifyPermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public ModifyPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public ModifyPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}