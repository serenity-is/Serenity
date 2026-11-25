namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this type should generate a columns script,
/// which contains information about properties in this type and
/// is an array of PropertyItem objects. Column scripts can be
/// accessed from the client side using Serenity.getColumns("Key").
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ColumnsScriptAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnsScriptAttribute"/> class.
    /// The key of the columns script will be the full name of the type this is placed on.
    /// </summary>
    /// <exception cref="ArgumentNullException">key</exception>
    public ColumnsScriptAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnsScriptAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <exception cref="ArgumentNullException">key</exception>
    public ColumnsScriptAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        Key = key;
    }

    /// <summary>
    /// Gets the key. If no key is set, and InferKey is true, the key will be inferred automatically 
    /// from the type name, module name and namespace. If InferKey is false, the key
    /// is full type name of the type this attribute is placed on.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string? Key { get; private set; }

    /// <summary>
    /// Indicates whether the key should be inferred automatically from the type name,
    /// module name and namespace.
    /// </summary>
    public bool InferKey { get; set; }

    /// <summary>
    /// If set, modifies the local text prefix that should be used
    /// instead of the automatically calculated one from Key and
    /// Namespace
    /// </summary>
    public string? LocalTextPrefix { get; set; }

    /// <summary>
    /// Optional revision number for the columns layout. Changing this value
    /// resets persisted personal grid column layouts from different revisions on restore.
    /// 0 means no revision.
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// Permission required to access the columns script. If not set, the script is public, unless the
    /// PropertyItemsScripts:PublicByDefault option is false, in which case the ReadPermission of the row type
    /// in its BasedOnRow attribute is used (DENY if the row has none), or PropertyItemsScripts:FallbackPermission
    /// (DENY by default) if there is no BasedOnRow attribute.
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Gets the automatically inferred columns key for the specified type.
    /// </summary>
    /// <param name="type">The type for which to infer the columns key.</param>
    /// <returns>The inferred columns key as a string.</returns>
    public static string AutoColumnsKeyFor(Type type)
    {
        return FormScriptAttribute.AutoFormKeyFor(type); // Columns key is inferred the same way as form key
    }
}