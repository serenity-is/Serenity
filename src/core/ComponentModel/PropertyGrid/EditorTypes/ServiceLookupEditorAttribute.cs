using Serenity.Navigation;

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
    /// Initializes a new instance of the <see cref="ServiceLookupEditorAttribute"/> class
    /// for use by derived editor attributes.
    /// </summary>
    protected ServiceLookupEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="service">The service, e.g. Northwind/Customer/List.</param>
    /// <param name="idField">Id field.</param>
    /// <param name="textField">Text field.</param>
    public ServiceLookupEditorAttribute(string service, string idField, string textField)
        : base(Key)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        IdField = idField ?? throw new ArgumentNullException(nameof(idField));
        TextField = textField ?? throw new ArgumentNullException(nameof(textField));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ServiceLookupEditorAttribute"/> class
    /// with an item/row type. Service, ID, and text fields are inferred when possible.
    /// For non-row types, inference uses <c>[IdProperty]</c> and <c>[NameProperty]</c>,
    /// then conventional property names
    /// (Id/Key/Code and Name/DisplayName/Text). A type with a single public string
    /// property uses that property for both ID and text. Otherwise, set
    /// <see cref="ServiceLookupEditorBaseAttribute.IdField"/> and
    /// <see cref="ServiceLookupEditorBaseAttribute.TextField"/> explicitly or annotate
    /// the corresponding item properties.
    /// </summary>
    /// <param name="itemType">The item/row type</param>
    public ServiceLookupEditorAttribute(Type itemType)
        : base(Key)
    {
        ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
    }

    /// <summary>
    /// When service is null, this method first tries to determine the service URL
    /// from a matching endpoint for row types, then falls back to the module identifier
    /// and type name convention.
    /// </summary>
    /// <param name="itemType">Type to generate a service for.</param>
    /// <param name="actionName">Optional action name.</param>
    /// <returns>Auto generated service.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemType"/> is null.</exception>
    public static string AutoServiceFor(Type itemType, string? actionName = null)
    {
        if (itemType is null)
            throw new ArgumentNullException(nameof(itemType));

        if (IsRowType(itemType) &&
            TryGetEndpointForRow(itemType, actionName, out string? service) is not null &&
                service is string fromRow)
                return fromRow;

        return AutoServiceByConvention(itemType, actionName);
    }

    private static string AutoServiceByConvention(Type type, string? actionName = null)
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
            else if (module.EndsWith(".Endpoints"))
                module = module[0..^10];

            var idx = module.IndexOf(".");
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        if (name.EndsWith("Row"))
            name = name[0..^3];
        else if (name.EndsWith("Lookup"))
            name = name[0..^6];
        else if (name.EndsWith("Endpoint"))
            name = name[0..^8];
        else if (name.EndsWith("Controller"))
            name = name[0..^10];

        return (string.IsNullOrEmpty(module) ? name :
            module.Replace('.', '/') + "/" + name) + "/" + (actionName ?? "List");
    }

    private static Type? TryGetEndpointForRow(Type rowType, string? actionName, out string? service)
    {
        service = null;
        var rowName = rowType.Name.EndsWith("Row", StringComparison.Ordinal) ?
            rowType.Name[..^3] : rowType.Name;
        var rowNamespace = rowType.Namespace ?? "";

        if (rowNamespace.EndsWith(".Entities", StringComparison.Ordinal))
            rowNamespace = rowNamespace[..^9];

        var endpointNames = new[]
        {
            string.IsNullOrEmpty(rowNamespace) ?
                $"Endpoints.{rowName}Endpoint" : $"{rowNamespace}.Endpoints.{rowName}Endpoint",
            string.IsNullOrEmpty(rowNamespace) ?
                $"{rowName}Endpoint" : $"{rowNamespace}.{rowName}Endpoint"
        };

        foreach (var endpointName in endpointNames)
        {
            var endpoint = rowType.Assembly.GetType(endpointName, throwOnError: false);
            if (endpoint is null ||
                !HasConnectionKeyForRow(endpoint, rowType))
                continue;

            service = TryGetServiceFromEndpoint(endpoint, actionName);
            if (service is not null)
                return endpoint;
        }
        return null;
    }

    /// <summary>
    /// Tries to get the service URL from an endpoint type and action name.
    /// </summary>
    /// <param name="endpoint">The endpoint type</param>
    /// <param name="actionName">The action name</param>
    /// <returns>The service URL if found; otherwise, null</returns>
    public static string? TryGetServiceFromEndpoint(Type endpoint, string? actionName)
    {
        if (actionName is null)
        {
            actionName = "ListLookup";
            if (!NavigationItemAttribute.HasRouteForAction(endpoint, actionName))
                actionName = "List";
        }

        if (!NavigationItemAttribute.HasRouteForAction(endpoint, actionName) ||
            NavigationItemAttribute.HasComplexRouteTemplate(endpoint, actionName))
            return null;

        var serviceUrl = NavigationItemAttribute.GetUrlFromController(
            endpoint, actionName, throwIfAbsent: false);
        if (serviceUrl is null)
            return null;

        if (!NavigationItemAttribute.HasActionRoute(endpoint, actionName))
            serviceUrl = AppendAction(serviceUrl, actionName);

        var servicePath = serviceUrl;
        if (servicePath.StartsWith("~/", StringComparison.Ordinal))
            servicePath = servicePath[2..];
        else if (servicePath.StartsWith("/", StringComparison.Ordinal))
            servicePath = servicePath[1..];

        if (servicePath.StartsWith("Services/", StringComparison.OrdinalIgnoreCase))
        {
            servicePath = servicePath["Services/".Length..];
            if (servicePath.Length > 0)
                return servicePath;
        }
        else if (servicePath.Length > 0)
            return "~/" + servicePath;

        return null;
    }

    private static bool IsRowType(Type type) =>
        type.FullName == "Serenity.Data.IRow" ||
        type.GetInterfaces().Any(@interface => @interface.FullName == "Serenity.Data.IRow");

    private static bool HasConnectionKeyForRow(Type endpoint, Type rowType)
    {
        for (var currentType = endpoint; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.GetCustomAttributesData().Any(attribute =>
                attribute.AttributeType.Name == "ConnectionKeyAttribute" &&
                attribute.AttributeType.Namespace == "Serenity.Data" &&
                attribute.ConstructorArguments.Any(argument =>
                    argument.ArgumentType == typeof(Type) &&
                    argument.Value is Type sourceType &&
                    sourceType == rowType)))
                return true;
        }

        return false;
    }

    private static string AppendAction(string serviceUrl, string actionName)
    {
        serviceUrl = serviceUrl.TrimEnd('/');
        var actionSuffix = "/" + actionName;
        if (serviceUrl.EndsWith(actionSuffix, StringComparison.OrdinalIgnoreCase))
            return serviceUrl;

        return serviceUrl + actionSuffix;
    }
}