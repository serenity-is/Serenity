using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class ImplicitBehaviorRegistry : IImplicitBehaviorRegistry
    {
        private readonly IEnumerable<Type> behaviorTypes;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="behaviorTypes">Types with IImplicitBehavior interface</param>
        public ImplicitBehaviorRegistry(IEnumerable<Type> behaviorTypes)
        {
            this.behaviorTypes = behaviorTypes ?? throw new ArgumentNullException(nameof(behaviorTypes));
        }

        /// <summary>
        /// Gets the implicit behavior types within provided assemblies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> FindImplicitBehaviorTypes(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(IImplicitBehavior).IsAssignableFrom(type));
        }

        public IEnumerable<Type> GetTypes()
        {
            return behaviorTypes;
        }
    }
}