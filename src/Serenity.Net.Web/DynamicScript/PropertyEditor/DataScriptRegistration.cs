using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using Serenity.ComponentModel;
using System;
using System.Reflection;

namespace Serenity.Web
{
    public class DataScriptRegistration
    {
        public static void RegisterDataScripts(IDynamicScriptManager scriptManager,
            ITypeSource typeSource, IServiceProvider serviceProvider)
        {
            if (scriptManager == null)
                throw new ArgumentNullException(nameof(scriptManager));

            if (typeSource == null)
                throw new ArgumentNullException(nameof(typeSource));

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            DataScriptAttribute attr;

            foreach (var type in typeSource.GetTypes())
            {
                if (type.IsAbstract ||
                    type.IsInterface ||
                    type.IsGenericTypeDefinition ||
                    !type.IsPublic)
                    continue;

                attr = type.GetCustomAttribute<DataScriptAttribute>();
                if (attr != null)
                {
                    var script = (INamedDynamicScript)ActivatorUtilities
                        .CreateInstance(serviceProvider, type);
                    scriptManager.Register(script);
                    continue;
                }
            }
        }
    }
}
