using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public class FormScriptRegistration
    {
        public static void RegisterFormScripts(IDynamicScriptManager scriptManager, IPropertyItemProvider registry, IEnumerable<Assembly> assemblies)
        {
            if (scriptManager == null)
                throw new ArgumentNullException(nameof(scriptManager));

            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var scripts = new List<Func<string>>();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<FormScriptAttribute>();
                    if (attr != null)
                    {
                        var script = new FormScript(attr.Key, type, registry);
                        scriptManager.Register(script);
                        scripts.Add(script.GetScript);
                    }
                }

            scriptManager.Register("FormBundle", new ConcatenatedScript(scripts));
        }
    }
}
