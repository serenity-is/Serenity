using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Contains registration methods for <see cref="ColumnsScript"/> types.
/// </summary>
public class ColumnsScriptRegistration
{
    /// <summary>
    /// Creates and registers column scripts.
    /// </summary>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="typeSource">The type source.</param>
    /// <param name="propertyProvider">The property item provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The list of registered column scripts.</returns>
    /// <exception cref="ArgumentNullException">Script manager, type source or service provider is <c>null</c>.</exception>
    public static IEnumerable<ColumnsScript> RegisterColumnsScripts(IDynamicScriptManager scriptManager,
        ITypeSource typeSource, IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(scriptManager);
        ArgumentNullException.ThrowIfNull(typeSource);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var options = serviceProvider.GetService<IOptions<PropertyItemsScriptOptions>>()?.Value ??
            new PropertyItemsScriptOptions();

        var scripts = new List<ColumnsScript>();
        foreach (var type in typeSource.GetTypesWithAttribute(typeof(ColumnsScriptAttribute)))
        {
            var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
            if (attr == null)
                continue;
            var key = attr.Key ?? (attr.InferKey ? ColumnsScriptAttribute.AutoColumnsKeyFor(type) : type.FullName!);
            var script = new ColumnsScript(key, type, propertyProvider, serviceProvider)
            {
                Permission = options.GetPermissionFor(type, attr.Permission)
            };
            scriptManager.Register(script);
            scripts.Add(script);
        }

        scriptManager.Register("ColumnsBundle", new PropertyItemsBundleScript(scripts,
            serviceProvider.GetService<IPermissionService>()));

        return scripts;
    }
}
