namespace Serenity.Data;

/// <summary>
/// An abstract base attribute that all permission related attributes derive from.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="PermissionAttributeBase"/> class.
/// </remarks>
/// <param name="permission">The permission.</param>
public abstract class PermissionAttributeBase(object? permission) : Attribute
{

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttributeBase"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public PermissionAttributeBase(object? module, object? permission)
        : this(module?.ToString() + ":" + permission)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttributeBase"/> class.
    /// A colon is inserted between module, submodule and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="submodule">The submodule.</param>
    /// <param name="permission">The permission.</param>
    public PermissionAttributeBase(object? module, object? submodule, object? permission)
        : this(module?.ToString() + ":" + submodule + ":" + permission)
    {
    }

    /// <summary>
    /// Gets the permission.
    /// </summary>
    /// <value>
    /// The permission.
    /// </value>
    public string? Permission { get; private set; } = permission?.ToString();
}