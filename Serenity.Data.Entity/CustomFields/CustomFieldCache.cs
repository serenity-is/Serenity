using System;
using System.Collections.Generic;
using Serenity.Data;
using Serenity.Services;
using System.Linq;

namespace Serenity.Data
{
    using TableAndGroup = Tuple<string, Int32>;

    public class CustomFieldCache
    {
        private static Dictionary<string, Dictionary<TableAndGroup, CacheItem>> bySchemaTableAndGroup;
        private static CacheItem empty;

        static CustomFieldCache()
        {
            empty = new CacheItem();
            bySchemaTableAndGroup = new Dictionary<string, Dictionary<TableAndGroup, CacheItem>>();

            SchemaChangeSource.Observers += (schema, table) => 
            {
                if (bySchemaTableAndGroup.ContainsKey(schema))
                {
                    var newBySchemaTableAndGroup = new Dictionary<string, Dictionary<TableAndGroup, CacheItem>>(bySchemaTableAndGroup);
                    newBySchemaTableAndGroup.Remove(schema);
                    bySchemaTableAndGroup = newBySchemaTableAndGroup;
                }
            };
        }

        public static IList<ICustomFieldDefinition> GetGroup(string schema, string table, Int32 group)
        {
            var byTableAndGroup = EnsureItems(schema);

            CacheItem item;
            if (!byTableAndGroup.TryGetValue(new TableAndGroup(table, group), out item))
                return empty.inDisplayOrder;

            return item.inDisplayOrder;
        }

        internal static void Refresh()
        {
            bySchemaTableAndGroup = null;
        }

        private static Dictionary<TableAndGroup, CacheItem> EnsureItems(string schema)
        {
            Dictionary<TableAndGroup, CacheItem> byTableAndGroup;
            if (bySchemaTableAndGroup.TryGetValue(schema, out byTableAndGroup))
                return byTableAndGroup;

            var temp = new Dictionary<TableAndGroup, CacheItem>();

            var rows = Dependency.Resolve<ICustomFieldListService>(schema).List(schema).ToList();
            rows.Sort((x, y) =>
            {
                var c = x.Table.CompareTo(y.Table);
                if (c == 0)
                    c = x.FieldGroup.CompareTo(y.FieldGroup);
                if (c == 0)
                    c = x.DisplayOrder.CompareTo(y.DisplayOrder);
                if (c == 0)
                    c = x.Id.CompareTo(y.Id);

                return c;
            });

            string priorTable = null;
            int priorGroup = Int32.MinValue;

            CacheItem item = null;

            foreach (var row in rows)
            {
                if (priorTable == null ||
                    priorTable != row.Table ||
                    priorGroup != row.FieldGroup)
                {
                    var key = new TableAndGroup(row.Table, row.FieldGroup);

                    if (!temp.TryGetValue(key, out item))
                    {
                        item = new CacheItem();
                        temp[key] = item;
                    }
                }

                item.inDisplayOrder.Add(row);
            }

            var newBySchemaTableAndGroup = new Dictionary<string, Dictionary<TableAndGroup, CacheItem>>(bySchemaTableAndGroup, StringComparer.OrdinalIgnoreCase);
            newBySchemaTableAndGroup[schema] = temp;

            bySchemaTableAndGroup = newBySchemaTableAndGroup;
            return temp;
        }

        public class CacheItem
        {
            internal CacheItem()
            {
                byName = new Dictionary<string, ICustomFieldDefinition>();
                inDisplayOrder = new List<ICustomFieldDefinition>();
            }

            internal Dictionary<string, ICustomFieldDefinition> byName;
            internal List<ICustomFieldDefinition> inDisplayOrder;

            public IList<ICustomFieldDefinition> InDisplayOrder
            {
                get { return inDisplayOrder; }
            }

            public ICustomFieldDefinition this[string valueKey]
            {
                get
                {
                    ICustomFieldDefinition item;
                    if (byName.TryGetValue(valueKey, out item))
                        return item;
                    return null;
                }
            }
        }
    }
}