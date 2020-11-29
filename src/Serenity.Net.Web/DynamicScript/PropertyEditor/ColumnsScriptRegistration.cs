using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Web
{
    public class ColumnsScriptRegistration
    {
        public static void RegisterColumnsScripts(IDynamicScriptManager scriptManager, IPropertyItemProvider registry, 
            ITypeSource typeSource)
        {
            if (scriptManager == null)
                throw new ArgumentNullException(nameof(scriptManager));

            if (typeSource == null)
                throw new ArgumentNullException(nameof(typeSource));

            var scripts = new List<Func<string>>();

            foreach (var type in typeSource.GetTypesWithAttribute(typeof(ColumnsScriptAttribute)))
            {
                var attr = type.GetCustomAttribute<ColumnsScriptAttribute>();
                var script = new ColumnsScript(attr.Key, type, registry);
                scriptManager.Register(script);
                scripts.Add(script.GetScript);
            }

            scriptManager.Register("ColumnsBundle", new ConcatenatedScript(scripts));
        }
    }
}
