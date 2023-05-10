
namespace Serenity.Data;

/// <summary>
/// Sets read permission for the row.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class ReadPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public ReadPermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public ReadPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public ReadPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}