namespace Serenity.Data;

/// <summary>
/// An abstract base attribute that all permission-related attributes derive from.
/// </summary>
/// <seealso cref="Attribute" />
public abstract class PermissionAttributeBase : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttributeBase"/> class.
    /// </summary>
    /// <param name="permission">The permission key.</param>
    /// <exception cref="ArgumentException"><paramref name="permission"/> is null, empty or whitespace.</exception>
    public PermissionAttributeBase(object? permission)
    {
        Permission = RequireKey(permission, nameof(permission));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionAttributeBase"/> class.
    /// A colon is inserted between module and permission to generate permission key.
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="permission">The permission.</param>
    public PermissionAttributeBase(object? module, object? permission)
        : this(Join(module, permission))
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
        : this(Join(module, submodule, permission))
    {
    }

    /// <summary>
    /// Gets the permission key. Never null or empty; use <see cref="SpecialPermissionKeys"/>
    /// for the special keys.
    /// </summary>
    /// <value>
    /// The permission.
    /// </value>
    public string Permission { get; }

    private static string RequireKey(object? value, string paramName)
    {
        var key = value?.ToString();
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(
                "Permission key can't be null or empty. Use SpecialPermissionKeys.Public, " +
                "SpecialPermissionKeys.LoggedIn or SpecialPermissionKeys.Deny for the special keys.", paramName);

        return key;
    }

    private static object Join(params object?[] parts)
    {
        return string.Join(":", parts.Select(part => part?.ToString()));
    }
}
