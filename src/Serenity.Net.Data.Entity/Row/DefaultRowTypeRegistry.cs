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

        public DefaultRowTypeRegistry(IEnumerable<Type> rowTypes)
        {
            this.rowTypes = rowTypes ?? throw new ArgumentNullException(nameof(rowTypes));
            byConnectionKey = this.rowTypes.Where(x => x.GetCustomAttribute<ConnectionKeyAttribute>() != null)
                .ToLookup(x => x.GetCustomAttribute<ConnectionKeyAttribute>().Value);
        }

        public IEnumerable<Type> AllRowTypes => rowTypes;

        public IEnumerable<Type> ByConnectionKey(string connectionKey)
        {
            return byConnectionKey[connectionKey];
        }

        public static IEnumerable<Type> EnumerateRowTypes(IEnumerable<Assembly> assemblies, 
            string connectionKey = null)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return assemblies.SelectMany(x => x.GetTypes())
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(IRow)) &&
                    (connectionKey == null || string.Compare(
                        type.GetCustomAttribute<ConnectionKeyAttribute>()?.Value,
                        connectionKey, StringComparison.OrdinalIgnoreCase) == 0));
        }
    }
}