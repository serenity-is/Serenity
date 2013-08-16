using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Serenity.Data;
using Serenity.Reflection;

namespace Serenity.Data
{
    public static class RowRegistry
    {
        public const string DefaultSchema = "Default";

        private static IDictionary<string, Row> emptySchema = new Dictionary<string, Row>(StringComparer.OrdinalIgnoreCase);
        internal static Dictionary<string, Dictionary<string, Row>> registry;

        static RowRegistry()
        {
            SchemaChangeSource.Observers += (schema, table) => {
                registry = null;
            };
        }

        public static string GetSchemaName(Type type)
        {
            var schemaAttrs = type.GetCustomAttributes(typeof(SchemaAttribute), true);
            if (schemaAttrs.Length == 1)
                return ((SchemaAttribute)schemaAttrs[0]).Schema;
            else
                return DefaultSchema;
        }

        public static string GetSchemaName(Row row)
        {
            return GetSchemaName(row.GetType());
        }

        public static IDictionary<string, Row> GetSchema(string schema)
        {
            var registry = EnsureRegistry();

            Dictionary<string, Row> schemaRegistry;
            if (registry.TryGetValue(schema, out schemaRegistry))
                return schemaRegistry;

            return emptySchema;
        }

        public static Row GetSchemaRow(string schema, string table)
        {
            var schemaRegistry = GetSchema(schema);
            Row row;
            if (schemaRegistry.TryGetValue(table, out row))
                return row;

            return null;
        }

        public static IEnumerable<Row> EnumerateRows()
        {
            var registry = EnsureRegistry();

            foreach (var schema in registry.Values)
                foreach (var row in schema.Values)
                    yield return row;
        }

        private static void AddInstance(Dictionary<string, Dictionary<string, Row>> registry, Row row)
        {
            try
            {
                var schemaName = GetSchemaName(row.GetType());
                Dictionary<string, Row> schemaRegistry;
                if (!registry.TryGetValue(schemaName, out schemaRegistry))
                    registry[schemaName] = schemaRegistry = new Dictionary<string, Row>(StringComparer.OrdinalIgnoreCase);

                string table = row.Table;

                schemaRegistry.Add(table, row);
            }
            catch (Exception ex)
            {
                new InvalidOperationException(String.Format("Can't register Row instance in DataSchema: {0}",
                    row.GetType().FullName), ex).Log();
            }
        }

        private static Row GetInstance(Type rowType)
        {
            //var instanceField = rowType.GetField("Instance",
            //    System.Reflection.BindingFlags.Public |
            //    System.Reflection.BindingFlags.Static);

            //if (instanceField != null)
            //    return (Row)instanceField.GetValue(null);
            
            //var instanceProp = rowType.GetProperty("Instance",
            //        System.Reflection.BindingFlags.Public |
            //        System.Reflection.BindingFlags.Static);

            //if (instanceProp != null)
            //    return (Row)instanceProp.GetValue(null, null);

            return (Row)InstanceCreator.GetInstance(rowType);
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