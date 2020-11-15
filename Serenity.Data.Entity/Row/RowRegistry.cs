using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    public static class RowRegistry
    {
        public static IEnumerable<Type> EnumerateRowTypes(IEnumerable<Assembly> assemblies, string connectionKey = null)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return assemblies.SelectMany(x => x.GetTypes())
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(IRow)) && 
                    (connectionKey == null || string.Compare(type.GetCustomAttribute<ConnectionKeyAttribute>()?.Value,
                        connectionKey, StringComparison.OrdinalIgnoreCase) == 0));
        }

        public static IEnumerable<IRow> EnumerateRowInstances(IEnumerable<Assembly> assemblies, string connectionKey = null)
        {
            return EnumerateRowTypes(assemblies, connectionKey).Select(type => (IRow)Activator.CreateInstance(type));
        }
    }
}