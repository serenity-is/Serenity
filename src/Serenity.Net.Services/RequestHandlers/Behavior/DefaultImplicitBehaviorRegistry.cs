#if TODO
using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class DefaultImplicitBehaviorRegistry : IImplicitBehaviorRegistry
    {
        public static readonly DefaultImplicitBehaviorRegistry Instance = new DefaultImplicitBehaviorRegistry();

        private static IEnumerable<Type> types;

        protected DefaultImplicitBehaviorRegistry()
        {
        }

        public virtual IEnumerable<Type> GetTypes()
        {
            if (types != null)
                return types;

            var list = new List<Type>();

            foreach (var type in ExtensibilityHelper.GetTypesWithInterface(typeof(IImplicitBehavior)))
                if (!type.IsAbstract)
                    list.Add(type);

            types = list;
            return types;
        }
    }
}
#endif