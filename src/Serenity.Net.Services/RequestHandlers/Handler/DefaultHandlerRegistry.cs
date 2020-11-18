using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public abstract class DefaultHandlerRegistry : IDefaultHandlerRegistry
    {
        private readonly IEnumerable<Type> types;

        protected DefaultHandlerRegistry(IEnumerable<Type> types)
        {
            this.types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public virtual IEnumerable<Type> GetTypes()
        {
            return types;
        }

        public static IEnumerable<Type> FindHandlerTypes(Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsInterface &&
                    !type.IsAbstract &&
                    typeof(IRequestHandler).IsAssignableFrom(type));
        }

        public IEnumerable<Type> GetTypes(Type handlerType)
        {
            return types.Where(type => handlerType.IsAssignableFrom(type));
        }
    }
}