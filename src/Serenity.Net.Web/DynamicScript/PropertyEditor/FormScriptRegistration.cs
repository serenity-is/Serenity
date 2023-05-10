using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Contains registration methods for <see cref="FormScript"/> types
/// </summary>
public class FormScriptRegistration
{
    /// <summary>
    /// Creates and form scripts
    /// </summary>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="typeSource">Type source</param>
    /// <param name="propertyProvider">Property item provider</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">Script manager or type source is null</exception>
    public static void RegisterFormScripts(IDynamicScriptManager scriptManager,
        ITypeSource typeSource, IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
    {
        if (scriptManager == null)
            throw new ArgumentNullException(nameof(scriptManager));

        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var scripts = new List<Func<string>>();

        foreach (var type in typeSource.GetTypesWithAttribute(typeof(FormScriptAttribute)))
        {
            var attr = type.GetCustomAttribute<FormScriptAttribute>();
            var key = attr.Key ?? type.FullName;
            var script = new FormScript(key, type, propertyProvider, serviceProvider);
            scriptManager.Register(script);
            scripts.Add(script.GetScript);
        }

        scriptManager.Register("FormBundle", new ConcatenatedScript(scripts));
    }
}
