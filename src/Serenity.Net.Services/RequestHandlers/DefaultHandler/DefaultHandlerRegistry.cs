#if TODO
using Serenity.Extensibility;
using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class DefaultHandlerRegistry : IDefaultHandlerRegistry
    {
        public static readonly DefaultHandlerRegistry Instance = new DefaultHandlerRegistry();

        private static List<Type> types;

        protected DefaultHandlerRegistry()
        {
        }

        public virtual IEnumerable<Type> GetTypes()
        {
            var types = DefaultHandlerRegistry.types;
            if (types != null)
                return types;

            types = new List<Type>();

            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                    if (!type.IsInterface &&
                        !type.IsAbstract &&
                        type.GetAttribute<DefaultHandlerAttribute>() != null)
                        types.Add(type);
            }

            DefaultHandlerRegistry.types = types;
            return types;
        }
    }
}
#endif