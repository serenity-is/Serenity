using Serenity.PropertyGrid;

namespace Serenity.Web
{
    public class ColumnsScriptRegistration
    {
        public static void RegisterColumnsScripts(IDynamicScriptManager scriptManager,
            ITypeSource typeSource, IPropertyItemProvider propertyProvider, IServiceProvider serviceProvider)
        {
            if (scriptManager == null)
                throw new ArgumentNullException(nameof(scriptManager));

            if (typeSource == null)
                throw new ArgumentNullException(nameof(typeSource));

            var scripts = new List<Func<string>>();

            foreach (var type in typeSource.GetTypesWithAttribute(typeof(ColumnsScriptAttribute)))
            {
                var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
                var key = attr.Key ?? type.FullName;
                var script = new ColumnsScript(key, type, propertyProvider, serviceProvider);
                scriptManager.Register(script);
                scripts.Add(script.GetScript);
            }

            scriptManager.Register("ColumnsBundle", new ConcatenatedScript(scripts));
        }
    }
}
