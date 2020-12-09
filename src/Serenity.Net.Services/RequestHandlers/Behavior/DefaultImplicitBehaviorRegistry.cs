using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public class DefaultImplicitBehaviorRegistry : IImplicitBehaviorRegistry
    {
        private readonly IEnumerable<Type> behaviorTypes;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="behaviorTypes">Types with IImplicitBehavior interface</param>
        public DefaultImplicitBehaviorRegistry(ITypeSource typeSource)
        {
            behaviorTypes = (typeSource ?? throw new ArgumentNullException(nameof(behaviorTypes)))
                .GetTypesWithInterface(typeof(IImplicitBehavior))
                .Where(type => !type.IsAbstract && !type.IsInterface);
        }

        public IEnumerable<Type> GetTypes()
        {
            return behaviorTypes;
        }
    }
}