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
                var script = new FormScript(attr.Key, type, propertyProvider, serviceProvider);
                scriptManager.Register(script);
                scripts.Add(script.GetScript);
            }

            scriptManager.Register("FormBundle", new ConcatenatedScript(scripts));
        }
    }
}
