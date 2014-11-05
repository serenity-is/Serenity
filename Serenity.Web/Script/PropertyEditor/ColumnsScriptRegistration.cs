using Serenity.ComponentModel;
using Serenity.Extensibility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public class ColumnsScriptRegistration
    {
        public static void RegisterColumnsScripts()
        {
            var assemblies = ExtensibilityHelper.SelfAssemblies;

            var scripts = new List<Func<string>>();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
                    if (attr != null)
                        scripts.Add(new ColumnsScript(attr.Key, type).GetScript);
                }

            DynamicScriptManager.Register("ColumnsBundle", new ConcatenatedScript(scripts));
        }
    }
}
