namespace Serenity.ComponentModel;

/// <summary>
/// Sets the enumeration key of an enum, or of a <see cref="StringEnum"/>, which is used as the
/// <c>Enums.&lt;EnumKey&gt;.&lt;Name&gt;</c> prefix of its member texts.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class, AllowMultiple = false)]
public class EnumKeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumKeyAttribute"/> class with the specified key.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EnumKeyAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumKeyAttribute"/> class with the key inferred
    /// from module attribute, if available and namespace.
    /// </summary>
    public EnumKeyAttribute()
    {
        InferKey = true;
    }

    /// <summary>
    /// Gets a value indicating whether the key should be inferred.
    /// </summary>
    public bool InferKey { get; private set; }

    /// <summary>
    /// Gets the enum key.
    /// </summary>
    /// <value>
    /// The enum key.
    /// </value>
    public string? Value { get; private set; }

    /// <summary>
    /// Automatically generates an enum key for the specified type based on its module and name.
    /// </summary>
    /// <param name="type">The type for which to generate the enum key.</param>
    /// <returns>The generated enum key.</returns>
    public static string AutoEnumKeyFor(Type type)
    {
        string module;
        var moduleAttr = type.GetCustomAttribute<ModuleAttribute>(true);

        if (moduleAttr != null)
            module = moduleAttr.Value;
        else
        {
            module = type.Namespace ?? "";

            if (module.EndsWith(".Entities", StringComparison.Ordinal))
                module = module[0..^9];
            else if (module.EndsWith(".Enums", StringComparison.Ordinal))
                module = module[0..^6];

            // skip the first part of the namespace (usually the root namespace)
            var idx = module.IndexOf(".");
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        return string.IsNullOrEmpty(module) ? name : module + "." + name;
    }
}