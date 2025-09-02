namespace Serenity.ComponentModel;

/// <summary>
/// Registers a permission key.
/// </summary>
/// <seealso cref="PermissionAttributeBase" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
public class RegisterPermissionKeyAttribute : PermissionAttributeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterPermissionKeyAttribute"/> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public RegisterPermissionKeyAttribute(object permission)
        : base(permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterPermissionKeyAttribute"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public RegisterPermissionKeyAttribute(object module, object permission)
        : base(module, permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterPermissionKeyAttribute"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public RegisterPermissionKeyAttribute(object module, object submodule, object permission)
        : base(module, submodule, permission)
    {
    }
}