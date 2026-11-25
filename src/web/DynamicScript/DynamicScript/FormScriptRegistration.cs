using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Contains registration methods for <see cref="FormScript"/> types.
/// </summary>
public class FormScriptRegistration
{
    /// <summary>
    /// Creates and registers form scripts.
    /// </summary>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="typeSource">The type source.</param>
    /// <param name="propertyProvider">The property item provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The list of registered form scripts.</returns>
    /// <exception cref="ArgumentNullException">Script manager, type source or service provider is <c>null</c>.</exception>
    public static IEnumerable<FormScript> RegisterFormScripts(IDynamicScriptManager scriptManager,
        ITypeSource typeSource, IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(scriptManager);
        ArgumentNullException.ThrowIfNull(typeSource);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var options = serviceProvider.GetService<IOptions<PropertyItemsScriptOptions>>()?.Value ??
            new PropertyItemsScriptOptions();

        var scripts = new List<FormScript>();
        foreach (var type in typeSource.GetTypesWithAttribute(typeof(FormScriptAttribute)))
        {
            var attr = type.GetCustomAttribute<FormScriptAttribute>();
            if (attr == null)
                continue;
            var key = attr.Key ?? (attr.InferKey ? FormScriptAttribute.AutoFormKeyFor(type) : type.FullName!);
            var script = new FormScript(key, type, propertyProvider, serviceProvider)
            {
                Permission = options.GetPermissionFor(type, attr.Permission)
            };
            scriptManager.Register(script);
            scripts.Add(script);
        }

        scriptManager.Register("FormBundle", new PropertyItemsBundleScript(scripts,
            serviceProvider.GetService<IPermissionService>()));

        return scripts;
    }
}
