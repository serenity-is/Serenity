using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Contains registration methods for <see cref="ColumnsScript"/> types
/// </summary>
public class ColumnsScriptRegistration
{
    /// <summary>
    /// Creates and registers column scripts
    /// </summary>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="typeSource">Type source</param>
    /// <param name="propertyProvider">Property item provider</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">Script manager or type source is null</exception>
    public static IEnumerable<ColumnsScript> RegisterColumnsScripts(IDynamicScriptManager scriptManager,
        ITypeSource typeSource, IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(scriptManager);
        ArgumentNullException.ThrowIfNull(typeSource);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var scripts = new List<ColumnsScript>();
        foreach (var type in typeSource.GetTypesWithAttribute(typeof(ColumnsScriptAttribute)))
        {
            var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
            var key = attr.Key ?? type.FullName;
            var script = new ColumnsScript(key, type, propertyProvider, serviceProvider);
            scriptManager.Register(script);
            scripts.Add(script);
        }

        scriptManager.Register("ColumnsBundle", new ConcatenatedScript(
        [
            () => PropertyItemsScript.Compact(scripts.Select(x => (x.ScriptName, (PropertyItemsData)x.GetScriptData())))
        ]));

        return scripts;
    }
}
