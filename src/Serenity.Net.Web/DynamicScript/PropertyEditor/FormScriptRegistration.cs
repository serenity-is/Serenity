using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public class FormScriptRegistration
    {
        public static void RegisterFormScripts(IDynamicScriptManager scriptManager, IPropertyItemProvider registry, 
            ITypeSource typeSource)
        {
            if (scriptManager == null)
                throw new ArgumentNullException(nameof(scriptManager));

            if (typeSource == null)
                throw new ArgumentNullException(nameof(typeSource));

            var scripts = new List<Func<string>>();

            foreach (var type in typeSource.GetTypesWithAttribute(typeof(FormScriptAttribute)))
            {
                var attr = type.GetCustomAttribute<FormScriptAttribute>();
                var script = new FormScript(attr.Key, type, registry);
                scriptManager.Register(script);
                scripts.Add(script.GetScript);
            }

            scriptManager.Register("FormBundle", new ConcatenatedScript(scripts));
        }
    }
}
