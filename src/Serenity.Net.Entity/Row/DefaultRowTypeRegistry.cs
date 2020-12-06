using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    public class DefaultRowTypeRegistry : IRowTypeRegistry
    {
        private readonly IEnumerable<Type> rowTypes;
        private readonly ILookup<string, Type> byConnectionKey;

        public DefaultRowTypeRegistry(ITypeSource typeSource)
        {
            rowTypes = (typeSource ?? throw new ArgumentNullException(nameof(typeSource)))
                .GetTypesWithInterface(typeof(IRow))
                .Where(x => !x.IsAbstract && !x.IsInterface);

            byConnectionKey = rowTypes.Where(x => x.GetCustomAttribute<ConnectionKeyAttribute>() != null)
                .ToLookup(x => x.GetCustomAttribute<ConnectionKeyAttribute>().Value);
        }

        public IEnumerable<Type> AllRowTypes => rowTypes;

        public IEnumerable<Type> ByConnectionKey(string connectionKey)
        {
            return byConnectionKey[connectionKey];
        }
    }
}