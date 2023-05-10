namespace Serenity.ComponentModel;

/// <summary>
/// Placed on rows / or custom lookup classes to denote
/// it has a lookup script.
/// When placed on a row class, lookup scripts only transfer ID 
/// and Name fields by default to client side for 
/// security / performance reasons. Make sure you add 
/// [LookupInclude] attribute to properties you'll need to
/// access from script.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class LookupScriptAttribute : Attribute
{
    /// <summary>
    /// Creates a LookupScriptAttribute with auto determined lookup key
    /// </summary>
    public LookupScriptAttribute()
    {
    }

    /// <summary>
    /// Creates a LookupScriptAttribute.
    /// </summary>
    /// <param name="key">Lookup key, usually in "Module.EntityName" format.</param>
    public LookupScriptAttribute(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("lookupKey");

        Key = key;
    }

    /// <summary>
    /// When lookup key is null, e.g. default constructor is used, this method
    /// tries to determine the lookup key by looking at the type this attribute 
    /// is placed on. This is a combination of module identifier and type name.
    /// If type has a [Module] attribute it is used, otherwise module identifier
    /// is determined from namespace, by removing ".Entities", ".Scripts", ".Lookups"
    /// common suffixes and the root namespace (e.g. the first part of namespace 
    /// before the first dot). Type name is determined from class type name, with
    /// common suffixes like "Row" or "Lookup" removed.
    /// </summary>
    /// <param name="type">Type to generate a lookup key for</param>
    /// <returns>Auto generated lookup key</returns>
    public static string AutoLookupKeyFor(Type type)
    {
        string module;
        var moduleAttr = type.GetCustomAttribute<ModuleAttribute>(true);
        if (moduleAttr != null)
            module = moduleAttr.Value;
        else
        {
            module = type.Namespace ?? "";

            if (module.EndsWith(".Entities"))
                module = module[0..^9];
            else if (module.EndsWith(".Scripts"))
                module = module[0..^8];
            else if (module.EndsWith(".Lookups"))
                module = module[0..^8];

            var idx = module.IndexOf(".");
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        if (name.EndsWith("Row"))
            name = name[0..^3];
        else if (name.EndsWith("Lookup"))
            name = name[0..^6];

        return string.IsNullOrEmpty(module) ? name :
            module + "." + name;
    }

    /// <summary>
    /// Defines that this type has an external lookup script type,
    /// and the lookup key is available on that type. Use this overload only
    /// for row types that have external lookups.
    /// </summary>
    /// <param name="lookupType">Script type with LookupScript attribute of its own.</param>
    public LookupScriptAttribute(Type lookupType)
    {
        if (lookupType == null)
            throw new ArgumentNullException("lookupType");

        var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>();
        if (attr == null)
            throw new ArgumentOutOfRangeException("lookupType", string.Format(
                "Type {0} is specified as lookup type in a LookupScript attribute, " +
                "but it has not LookupScript attribute itself.", lookupType.FullName));

        Key = attr.Key ?? AutoLookupKeyFor(lookupType);
        LookupType = lookupType;
    }

    /// <summary>
    /// Lookup key, usually in "Module.EntityName" format.
    /// </summary>
    public string? Key { get; private set; }

    /// <summary>
    /// Permission key required to access this lookup script.
    /// Use special value "?" for all logged-in users.
    /// Use special value "*" for anyone including not logged-in users.
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Cache duration in seconds
    /// </summary>
    public int Expiration { get; set; }

    /// <summary>
    /// External lookup script type or base type that should be used for generating dynamic lookup script.
    /// Only meaningful for Row types with external lookup scripts. Can be a generic type of TRow or
    /// a simple lookup class.
    /// </summary>
    public Type? LookupType { get; set; }
}