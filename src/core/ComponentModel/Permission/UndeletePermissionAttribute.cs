
namespace Serenity.Data;

/// <summary>
/// Sets undelete permission for the row.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class UndeletePermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UndeletePermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public UndeletePermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UndeletePermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public UndeletePermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UndeletePermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public UndeletePermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}