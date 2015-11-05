using Serenity.Extensibility;
using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    public class DefaultImplicitBehaviorRegistry : IImplicitBehaviorRegistry
    {
        private static IEnumerable<Type> types;

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