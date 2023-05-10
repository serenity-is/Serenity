namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "ServiceLookup" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public class ServiceLookupEditorAttribute : ServiceLookupEditorBaseAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "ServiceLookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="service">The service, e.g. Northwind/Customer/List.</param>
    /// <param name="idField">Id field.</param>
    /// <param name="textField">Text field.</param>
    public ServiceLookupEditorAttribute(string service, string idField, string textField)
        : base(Key)
    {
        SetOption("service", service);
        SetOption("idField", idField);
        SetOption("textField", textField);
    }

    /// <summary>
    /// If you use this constructor, service will tried to be determined by module and
    /// name of the row class, and idField and textField will be determined by
    /// rows id and name fields.
    /// </summary>
    public ServiceLookupEditorAttribute(Type itemType)
        : base(Key)
    {
        ItemType = itemType ?? throw new ArgumentNullException("itemType");
    }

    /// <summary>
    /// When service is null, this method tries to determine the service by looking 
    /// at the type this attribute is placed on. This is a combination of module identifier and type name.
    /// If type has a [Module] attribute it is used, otherwise module identifier
    /// is determined from namespace, by removing ".Entities", ".Scripts", ".Lookups"
    /// common suffixes and the root namespace (e.g. the first part of namespace 
    /// before the first dot). Type name is determined from class type name, with
    /// common suffixes like "Row" or "Lookup" removed.
    /// </summary>
    /// <param name="type">Type to generate a service for</param>
    /// <returns>Auto generated service</returns>
    public static string AutoServiceFor(Type type)
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

        return (string.IsNullOrEmpty(module) ? name :
            module + "/" + name) + "/List";
    }
}