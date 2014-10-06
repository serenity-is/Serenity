using Serenity.ComponentModel;
using Serenity.Extensibility;
using System.Reflection;

namespace Serenity.Web
{
    public class ColumnsScriptRegistration
    {
        public static void RegisterColumnsScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
                    if (attr != null)
                        new ColumnsScript(attr.Key, type);
                }
        }
    }
}
