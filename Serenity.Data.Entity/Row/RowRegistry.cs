using System.Collections.Generic;
using System;
using Serenity.Extensibility;
using System.Linq;

namespace Serenity.Data
{
    public static class RowRegistry
    {
        public const string DefaultConnectionKey = "Default";

        private static ILookup<string, Row> emptyRegistry = new List<Row>().ToLookup(x => (string)null);
        internal static IDictionary<string, ILookup<string, Row>> registry;

        static RowRegistry()
        {
            SchemaChangeSource.Observers += (connectionKey, table) => {
                registry = null;
            };
        }

        public static ILookup<string, Row> ByConnectionKey(string connectionKey)
        {
            var registry = EnsureRegistry();
            ILookup<string, Row> connectionRegistry;
            if (!registry.TryGetValue(connectionKey, out connectionRegistry))
                return emptyRegistry;

            return connectionRegistry;
        }

        public static IEnumerable<Row> EnumerateRows()
        {
            var registry = EnsureRegistry();

            foreach (var reg in registry.Values)
                foreach (var rows in reg)
                    foreach (var row in rows)
                        yield return row;
        }

        private static Row GetInstance(Type rowType)
        {
            return (Row)Activator.CreateInstance(rowType);
        }

        private static IDictionary<string, ILookup<string, Row>> EnsureRegistry()
        {
            var reg = registry;
            if (reg == null)
            {
                registry = reg = Initialize();
                return reg;
            }

            return reg;
        }

        private static IDictionary<string, ILookup<string, Row>> Initialize()
        {
            var rows = new List<Row>();
            
            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (!type.IsAbstract &&
                        type.IsSubclassOf(typeof(Row)))
                    {
                        var instance = GetInstance(type);
                        rows.Add(instance);
                    }

            return rows.GroupBy(x => x.GetFields().ConnectionKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.ToLookup(z => z.Table, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }
    }
}