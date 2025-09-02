
namespace Serenity.Data;

/// <summary>
/// Sets a permission to read only lookup fields in a row via List service
/// Lookup field means ID, Name, and [LookupInclude] properties in a row.
/// You must use [AuthorizeList(typeof(XRow))] instead of ServiceAuthorize in 
/// service endpoint.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
public class ServiceLookupPermissionAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupPermissionAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public ServiceLookupPermissionAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupPermissionAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public ServiceLookupPermissionAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupPermissionAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public ServiceLookupPermissionAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}