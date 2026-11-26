using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.Localization;
using Serenity.PropertyGrid;
using System.Text.Json;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="ILocalTextPermissionFilter"/>, see
/// <see cref="LocalTextPermissionOptions.Enabled"/> for the rules.
/// </summary>
public class LocalTextPermissionFilter : ILocalTextPermissionFilter
{
    private readonly LocalTextPermissionOptions options;
    private readonly ITypeSource typeSource;
    private readonly IPermissionService permissions;
    private readonly IServiceProvider serviceProvider;
    private readonly IHttpContextAccessor? httpContextAccessor;
    private readonly Lazy<LocalTextPermissionModel> model;
    private readonly object itemsKey = new();

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="typeSource">Type source to find rows, forms, columns and nested local texts</param>
    /// <param name="permissions">Permission service</param>
    /// <param name="serviceProvider">Service provider, used to get property items of forms and columns</param>
    /// <param name="httpContextAccessor">HTTP context accessor, used to determine the permissions of
    /// the current user once per request</param>
    public LocalTextPermissionFilter(IOptions<LocalTextPermissionOptions> options, ITypeSource typeSource,
        IPermissionService permissions, IServiceProvider serviceProvider, IHttpContextAccessor? httpContextAccessor = null)
    {
        this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
        this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.httpContextAccessor = httpContextAccessor;
        model = new Lazy<LocalTextPermissionModel>(BuildModel);
    }

    /// <summary>
    /// Gets the model, which is built on first access.
    /// </summary>
    public LocalTextPermissionModel Model => model.Value;

    /// <summary>
    /// Returns true if texts should be filtered for the current user, e.g. filtering is enabled, and
    /// the user does not have the <see cref="LocalTextPermissionOptions.UnfilteredPermission"/>.
    /// </summary>
    protected virtual bool ShouldFilter()
    {
        return options.Enabled &&
            (string.IsNullOrEmpty(options.UnfilteredPermission) ||
             !permissions.HasPermission(options.UnfilteredPermission));
    }

    /// <inheritdoc/>
    public string GetCacheSuffix()
    {
        if (!ShouldFilter())
            return "";

        var granted = GetGranted();
        var bits = new byte[(granted.Length + 7) / 8];
        for (var i = 0; i < granted.Length; i++)
            if (granted[i])
                bits[i / 8] |= (byte)(1 << (i % 8));

        return "p" + Convert.ToBase64String(bits);
    }

    /// <inheritdoc/>
    public Func<string, bool>? GetKeyFilter()
    {
        if (!ShouldFilter())
            return null;

        var granted = GetGranted();
        var current = Model;
        return key => current.IsAllowed(key, granted);
    }

    private bool[] GetGranted()
    {
        var items = httpContextAccessor?.HttpContext?.Items;
        if (items != null && items.TryGetValue(itemsKey, out var cached) && cached is bool[] cachedGranted)
            return cachedGranted;

        var current = Model;
        var granted = new bool[current.Permissions.Length];
        for (var i = 0; i < granted.Length; i++)
            granted[i] = permissions.HasPermission(current.Permissions[i]);

        if (items != null)
            items[itemsKey] = granted;

        return granted;
    }

    /// <summary>
    /// Builds the model from rows, forms, columns and nested local texts classes.
    /// </summary>
    protected virtual LocalTextPermissionModel BuildModel()
    {
        var permissionIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var gates = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
        var grants = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
        var restrictions = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        int indexOf(string? permission)
        {
            permission = string.IsNullOrEmpty(permission) ? SpecialPermissionKeys.Deny : permission;
            if (!permissionIndexes.TryGetValue(permission, out var index))
                permissionIndexes[permission] = index = permissionIndexes.Count;
            return index;
        }

        static void add(Dictionary<string, HashSet<int>> target, string key, int index)
        {
            if (!target.TryGetValue(key, out var set))
                target[key] = set = [];
            set.Add(index);
        }

        static string withDot(string prefix) => prefix.EndsWith('.') ? prefix : prefix + ".";

        // row texts require the read permission of the row, and are denied when it has none
        foreach (var rowType in typeSource.GetTypesWithInterface(typeof(IRow)))
        {
            if (rowType.IsAbstract || rowType.IsInterface || rowType.ContainsGenericParameters ||
                rowType.GetConstructor(Type.EmptyTypes) is null)
                continue;

            string? localTextPrefix;
            try
            {
                localTextPrefix = (Activator.CreateInstance(rowType) as IRow)?.Fields.LocalTextPrefix;
            }
            catch
            {
                continue;
            }

            if (string.IsNullOrEmpty(localTextPrefix))
                continue;

            add(gates, "Db." + withDot(localTextPrefix),
                indexOf(rowType.GetCustomAttribute<ReadPermissionAttribute>(true)?.Permission));
        }

        // texts of a form or columns type require the permission of its script, and the texts its
        // property items use are allowed for users that can access it, e.g. a public form based on a row
        var propertyItemsOptions = serviceProvider.GetService<IOptions<PropertyItemsScriptOptions>>()?.Value ??
            new PropertyItemsScriptOptions();
        var propertyProvider = serviceProvider.GetService<IPropertyItemProvider>();
        foreach (var type in typeSource.GetTypesWithAttribute(typeof(FormScriptAttribute))
            .Concat(typeSource.GetTypesWithAttribute(typeof(ColumnsScriptAttribute))).Distinct())
        {
            var explicitPermission = type.GetCustomAttribute<FormScriptAttribute>()?.Permission ??
                type.GetCustomAttribute<ColumnsScriptAttribute>()?.Permission;
            var permission = propertyItemsOptions.GetPermissionFor(type, explicitPermission) ??
                SpecialPermissionKeys.Public;
            var index = indexOf(permission);

            if (PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(type) is string textPrefix &&
                textPrefix.Length > 0)
                add(gates, withDot(textPrefix), index);

            if (propertyProvider is null)
                continue;

            object data;
            try
            {
                data = new FormScript("LocalTextPermissions", type, propertyProvider, serviceProvider).GetScriptData();
            }
            catch
            {
                continue;
            }

            CollectTextKeys(JsonSerializer.SerializeToElement(data, JSON.Defaults.Strict), null,
                key => add(grants, key, index),
                enumKey => add(gates, "Enums." + withDot(enumKey), index));
        }

        // texts of nested local text classes with a permission require that permission, e.g. the texts of a module's
        // pages, or a few row captions a profile page uses for users without access to the row
        foreach (var type in typeSource.GetTypesWithAttribute(typeof(NestedLocalTextsAttribute)))
        {
            var attr = type.GetCustomAttribute<NestedLocalTextsAttribute>();
            if (string.IsNullOrEmpty(attr?.Permission))
                continue;

            var index = indexOf(attr.Permission);
            foreach (var key in GetNestedTextKeys(type, attr.Prefix ?? ""))
                add(restrictions, key, index);
        }

        // texts of an enum with a RequiredPermission or ReadPermission require that permission,
        // instead of depending on the forms and columns that use the enum
        foreach (var type in typeSource.GetTypes())
        {
            string? enumKey = type.IsEnum ? EnumMapper.GetEnumTypeKey(type) :
                StringEnum.IsStringEnum(type) ? StringEnum.GetInfo(type).EnumKey : null;
            if (string.IsNullOrEmpty(enumKey))
                continue;

            var permission = type.GetCustomAttribute<RequiredPermissionAttribute>(false)?.Permission ??
                type.GetCustomAttribute<ReadPermissionAttribute>(false)?.Permission;
            if (string.IsNullOrEmpty(permission))
                continue;

            gates["Enums." + withDot(enumKey)] = [indexOf(permission)];
        }

        // permission key texts are only needed by permission editors
        add(gates, "Permission.", indexOf(options.PermissionTextsPermission ?? SpecialPermissionKeys.LoggedIn));

        // prefixes restricted in options decide for the texts under them, instead of the rules above
        foreach (var pair in options.Restrict ?? [])
        {
            if (!string.IsNullOrEmpty(pair.Key))
                gates[withDot(pair.Key)] = [indexOf(pair.Value)];
        }

        var permissionList = new string[permissionIndexes.Count];
        foreach (var pair in permissionIndexes)
            permissionList[pair.Value] = pair.Key;

        return new LocalTextPermissionModel(permissionList,
            gates.ToDictionary(x => x.Key, x => x.Value.ToArray(), StringComparer.OrdinalIgnoreCase),
            grants.ToDictionary(x => x.Key, x => x.Value.ToArray(), StringComparer.OrdinalIgnoreCase),
            restrictions.ToDictionary(x => x.Key, x => x.Value.ToArray(), StringComparer.OrdinalIgnoreCase),
            [.. (options.AlwaysInclude ?? []).Where(x => !string.IsNullOrEmpty(x))]);
    }

    /// <summary>
    /// Collects the local text keys used in serialized property items, and the keys of the enums
    /// their editors, formatters or filters use.
    /// </summary>
    private static void CollectTextKeys(JsonElement element, string? propertyName,
        Action<string> addKey, Action<string> addEnumKey)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                    CollectTextKeys(property.Value, property.Name, addKey, addEnumKey);
                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                    CollectTextKeys(item, propertyName, addKey, addEnumKey);
                break;

            case JsonValueKind.String:
                var text = element.GetString();
                if (string.IsNullOrEmpty(text))
                    break;

                if (string.Equals(propertyName, "enumKey", StringComparison.OrdinalIgnoreCase))
                    addEnumKey(text);
                // any key like text, as a form may use texts outside the usual prefixes, while
                // a string that is not a text key here only adds a grant that is never used
                else if (LocalText.IsLocalTextKeyCandidate(text, uiKeyOnly: false))
                    addKey(text);
                break;
        }
    }

    /// <summary>
    /// Gets the keys of texts declared by a nested local texts class, the same way they are registered.
    /// </summary>
    private static IEnumerable<string> GetNestedTextKeys(Type type, string prefix)
    {
        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (field.FieldType != typeof(LocalText))
                continue;

            // registration replaces the key of a field with its prefixed name, keeping the default text as original key
            yield return field.GetValue(null) is LocalText text && !string.IsNullOrEmpty(text.Key) &&
                ((ILocalText)text).OriginalKey is not null ? text.Key : prefix + field.Name;
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var name = nested.Name.EndsWith('_') ? nested.Name[0..^1] : nested.Name;
            foreach (var key in GetNestedTextKeys(nested, prefix + name + "."))
                yield return key;
        }
    }
}
