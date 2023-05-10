
namespace Serenity.Data;

/// <summary>
/// Sets navigation permission for the row, which if exists takes 
/// precedence over ReadPermissionAttribute to determine permission for
/// row's page and navigation item.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class NavigationPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public NavigationPermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public NavigationPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public NavigationPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}