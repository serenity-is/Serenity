using Serenity.ComponentModel;
using Serenity.Extensibility;
using System.Reflection;

namespace Serenity.Web
{
    public class FormScriptRegistration
    {
        public static void RegisterFormScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<FormScriptAttribute>();
                    if (attr != null)
                        new FormScript(attr.Key, type);
                }
        }
    }
}
