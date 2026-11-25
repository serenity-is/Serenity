using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Abstract base class for <see cref="ColumnsScript"/> and <see cref="FormScript"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="scriptName">The script name.</param>
/// <param name="type">The columns or form type.</param>
/// <param name="propertyProvider">The property item provider.</param>
/// <param name="serviceProvider">The service provider.</param>
public abstract partial class PropertyItemsScript(string scriptName, Type type,
    IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider) : INamedDynamicScript, IGetScriptData
{
    private readonly Type type = type ?? throw new ArgumentNullException(nameof(type));
    private readonly IServiceProvider serviceProvider = serviceProvider ??
            throw new ArgumentNullException(nameof(serviceProvider));
    private readonly IPropertyItemProvider propertyProvider = propertyProvider ??
            throw new ArgumentNullException(nameof(PropertyItemsScript.propertyProvider));
    private EventHandler? scriptChanged;

    /// <summary>
    /// Checks the name if it is empty or null.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>The validated name.</returns>
    /// <exception cref="ArgumentNullException">name is null or empty.</exception>
    protected static string CheckName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        return name;
    }

    /// <inheritdoc/>
    public TimeSpan Expiration { get; set; }

    /// <inheritdoc/>
    public string? GroupKey { get; set; }

    /// <inheritdoc/>
    public void Changed()
    {
        scriptChanged?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc/>
    public string ScriptName => scriptName;

    /// <summary>
    /// Permission required to access this script. When null, the script is public.
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// Returns true if <see cref="Permission"/> is null, or the current user has it.
    /// </summary>
    /// <param name="permissions">Permission service</param>
    public bool HasPermission(IPermissionService permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        return Permission is null || permissions.HasPermission(Permission);
    }

    /// <summary>
    /// Gets the default permission for a form or columns type, which is the
    /// ReadPermission attribute on the row type specified with the BasedOnRow attribute.
    /// As with rows, if the row has no ReadPermission, access is denied.
    /// </summary>
    /// <param name="type">Form or columns type</param>
    /// <returns>The read permission of the row, <see cref="SpecialPermissionKeys.Deny"/>
    /// if the row has none, or null if the type has no BasedOnRow attribute</returns>
    public static string? GetDefaultPermission(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var rowType = type.GetCustomAttribute<BasedOnRowAttribute>(true)?.RowType;
        if (rowType is null)
            return null;

        var permission = rowType.GetCustomAttribute<ReadPermissionAttribute>(true)?.Permission;
        return string.IsNullOrEmpty(permission) ? SpecialPermissionKeys.Deny : permission;
    }

    /// <inheritdoc/>
    public string GetScript()
    {
        var data = GetScriptData();
        return string.Format(CultureInfo.InvariantCulture, DataScript.SetScriptDataFormat,
            scriptName.ToSingleQuoted(),
            JSON.Stringify(data, writeNulls: false));
    }

    /// <inheritdoc/>
    public object GetScriptData()
    {
        var data = new PropertyItemsData
        {
            Items = [.. propertyProvider.GetPropertyItemsFor(type)],
            AdditionalItems = []
        };

        var columnsAttr = type.GetCustomAttribute<ColumnsScriptAttribute>();
        if (columnsAttr?.Revision > 0)
            data.Revision = columnsAttr.Revision;

        // the views of the grid as a whole, e.g. a card view
        var recordViews = type.GetCustomAttributes<RecordViewTypeAttribute>(inherit: true).ToList();
        if (recordViews.Count > 0)
            data.RecordViews = [.. recordViews.Select(x => RecordViewInfo.FromAttribute(x))];

        if (typeof(ICustomizePropertyItems).IsAssignableFrom(type))
        {
            var instance = ActivatorUtilities.CreateInstance(
                serviceProvider, type) as ICustomizePropertyItems;
            instance!.Customize(data.Items);
        }

        var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
        if (basedOnRowAttr != null &&
            basedOnRowAttr.RowType != null)
        {
            var existing = new HashSet<string>(data.Items.Where(x => x.Name is not null).Select(x => x.Name!));
            var additional = new HashSet<string>();
            foreach (var item in data.Items)
            {
                if (!string.IsNullOrEmpty(item.FilteringIdField) &&
                    !existing.Contains(item.FilteringIdField))
                    additional.Add(item.FilteringIdField);
            }

            if (additional.Count > 0)
            {
                data.AdditionalItems = [.. propertyProvider.GetPropertyItemsFor(basedOnRowAttr.RowType,
                    property => additional.Contains(property.Name))];
            }
        }

        return data;
    }

    /// <inheritdoc/>
    public event EventHandler ScriptChanged
    {
        add { scriptChanged += value; }
        remove { scriptChanged -= value; }
    }
}