namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this type should generate a form script,
/// which contains information about properties in this type and
/// is an array of PropertyItem objects. Form scripts can be
/// accessed from the client side using Serenity.getForm("Key").
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FormScriptAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormScriptAttribute"/> class.
    /// The key of the form script will be the full name of the type this is placed on.
    /// </summary>
    /// <exception cref="ArgumentNullException">key</exception>
    public FormScriptAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormScriptAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <exception cref="ArgumentNullException">key</exception>
    public FormScriptAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("key");

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
    /// module name and namespace. For compat, this defaults to false, resulting
    /// in using the full type name as the key.
    /// </summary>
    public bool InferKey { get; set; }

    /// <summary>
    /// If set, modifies the local text prefix that should be used
    /// instead of the automatically calculated one from Key and
    /// Namespace
    /// </summary>
    public string? LocalTextPrefix { get; set; }

    /// <summary>
    /// Gets the automatically inferred form key for the specified type.
    /// </summary>
    /// <param name="type">The type for which to infer the form key.</param>
    /// <returns>The inferred form key as a string.</returns>
    public static string AutoFormKeyFor(Type type)
    {
        string module;
        var moduleAttr = type.GetCustomAttribute<ModuleAttribute>(true) ??
            type.GetCustomAttribute<BasedOnRowAttribute>(true)?.RowType?.GetCustomAttribute<ModuleAttribute>(true);

        if (moduleAttr != null)
            module = moduleAttr.Value;
        else
        {
            module = type.Namespace ?? "";

            if (module.EndsWith(".Forms", StringComparison.Ordinal))
                module = module[0..^6];
            else if (module.EndsWith(".Columns", StringComparison.Ordinal))
                module = module[0..^8];

            // skip the first part of the namespace (usually the root namespace)
            var idx = module.IndexOf(".");
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        if (name.EndsWith("Form", StringComparison.Ordinal))
            name = name[0..^4];
        else if (name.EndsWith("Columns", StringComparison.Ordinal))
            name = name[0..^7];
        else if (name.EndsWith("Request", StringComparison.Ordinal))
            name = name[0..^7];

        return string.IsNullOrEmpty(module) ? name : module + "." + name;
    }
}