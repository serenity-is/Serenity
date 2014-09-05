using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Serenity.Data;
using Serenity.Extensibility;
using Serenity.Reflection;

namespace Serenity.Data
{
    public static class RowRegistry
    {
        public const string DefaultConnectionKey = "Default";

        private static IDictionary<string, Row> emptyRegistry = new Dictionary<string, Row>(StringComparer.OrdinalIgnoreCase);
        internal static Dictionary<string, Dictionary<string, Row>> registry;

        static RowRegistry()
        {
            SchemaChangeSource.Observers += (connectionKey, table) => {
                registry = null;
            };
        }

        public static string GetConnectionKey(Type type)
        {
            var connectionKeyAttrs = type.GetCustomAttributes(typeof(ConnectionKeyAttribute), true);
            if (connectionKeyAttrs.Length == 1)
                return ((ConnectionKeyAttribute)connectionKeyAttrs[0]).Value;
            else
                return DefaultConnectionKey;
        }

        public static string GetConnectionKey(Row row)
        {
            return GetConnectionKey(row.GetType());
        }

        public static IDictionary<string, Row> GetRegistry(string connectionKey)
        {
            var registry = EnsureRegistry();

            Dictionary<string, Row> schemaRegistry;
            if (registry.TryGetValue(connectionKey, out schemaRegistry))
                return schemaRegistry;

            return emptyRegistry;
        }

        public static Row GetConnectionRow(string connectionKey, string table)
        {
            var connectionRegistry = GetRegistry(connectionKey);
            Row row;
            if (connectionRegistry.TryGetValue(table, out row))
                return row;

            return null;
        }

        public static IEnumerable<Row> EnumerateRows()
        {
            var registry = EnsureRegistry();

            foreach (var reg in registry.Values)
                foreach (var row in reg.Values)
                    yield return row;
        }

        private static void AddInstance(Dictionary<string, Dictionary<string, Row>> registry, Row row)
        {
            try
            {
                var connectionKey = GetConnectionKey(row.GetType());
                Dictionary<string, Row> connectionRegistry;
                if (!registry.TryGetValue(connectionKey, out connectionRegistry))
                    registry[connectionKey] = connectionRegistry = new Dictionary<string, Row>(StringComparer.OrdinalIgnoreCase);

                string table = row.Table;

                connectionRegistry.Add(table, row);
            }
            catch (Exception ex)
            {
                new InvalidOperationException(String.Format("Can't register Row instance in DataSchema: {0}",
                    row.GetType().FullName), ex).Log();
            }
        }

        private static Row GetInstance(Type rowType)
        {
            return (Row)Activator.CreateInstance(rowType);
        }

        private static Dictionary<string, Dictionary<string, Row>> EnsureRegistry()
        {
            var reg = registry;
            if (reg == null)
                reg = Initialize();

            registry = reg;
            return reg;
        }

        private static Dictionary<string, Dictionary<string, Row>> Initialize()
        {
            var newRegistry = new Dictionary<string, Dictionary<string, Row>>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var assembly in ExtensibilityHelper.SelfAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (!type.IsAbstract &&
                        type.IsSubclassOf(typeof(Row)))
                    {
                        var instance = GetInstance(type);
                        AddInstance(newRegistry, instance);
                    }

            return newRegistry;
        }
    }
}