using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public class DataEnumCache
    {
        private static List<DataEnumRow> _constantItems;
        private static Dictionary<string, CacheItem> _items;
        private static CacheItem _empty;

        static DataEnumCache()
        {
            _empty = new CacheItem();
        }

        public static CacheItem EnumType(string enumType)
        {
            EnsureItems();
            CacheItem item;
            if (!_items.TryGetValue(enumType, out item))
                return _empty;
            return item;
        }

        internal static void Refresh()
        {
            _items = null;
        }

        public static List<DataEnumRow> ConstantItems
        {
            get { return null; }
            set { _constantItems = value; }
        }

        private static void EnsureItems()
        {
            if (_items != null)
                return;

            var temp = new Dictionary<string, CacheItem>();

            IList<DataEnumRow> list;
            if (_constantItems != null)
                list = _constantItems;
            else
            {
                // bunu DI ile vs. bir şekilde halletmeliyiz
                throw new InvalidOperationException(); 
                /*
                using (var connection = SqlConnections.New())
                {
                    var row = new DataEnumRow();
                    var fld = DataEnumRow.Fields;

                    list = new SqlSelect().FromAsT0(row).Select(
                        fld.EnumType,
                        fld.ValueKey,
                        fld.ValueId)
                    .OrderBy(
                        fld.EnumType,
                        fld.DisplayOrder,
                        fld.ValueId)
                    .List(connection, row);
                }

                foreach (var r in list)
                {
                    string priorType = null;
                    CacheItem item = null;
                    var fld = DataEnumRow.Fields;

                    if (priorType == null ||
                        priorType != r.EnumType)
                    {
                        if (!temp.TryGetValue(r.EnumType, out item))
                        {
                            item = new CacheItem();
                            temp[r.EnumType] = item;
                        }
                    }

                    var de = new DataEnumItem(r.ValueId.Value, r.ValueKey);
                    item._byValueId[r.ValueId.Value] = de;
                    item._byValueKey[r.ValueKey] = de;
                    item._inDisplayOrder.Add(de);
                }

                _items = temp;*/
            }
        }

        public class CacheItem
        {
            internal CacheItem()
            {
                _byValueKey = new Dictionary<string,DataEnumItem>();
                _byValueId = new Dictionary<int,DataEnumItem>();
                _inDisplayOrder = new List<DataEnumItem>();
            }

            internal Dictionary<string, DataEnumItem> _byValueKey;
            internal Dictionary<int, DataEnumItem> _byValueId;
            internal List<DataEnumItem> _inDisplayOrder;

            public ICollection<DataEnumItem> InDisplayOrder
            {
                get { return _inDisplayOrder; }
            }

            public Int32? this[string valueKey]
            {
                get
                {
                    DataEnumItem item;
                    if (_byValueKey.TryGetValue(valueKey, out item))
                        return item.ValueId;
                    return null;
                }
            }

            public string this[int valueId]
            {
                get
                {
                    DataEnumItem item;
                    if (_byValueId.TryGetValue(valueId, out item))
                        return item.ValueKey;
                    return null;
                }
            }
        }
       
        public class DataEnumItem
        {
            public DataEnumItem(int valueId, string valueKey)
            {
                ValueId = valueId;
                ValueKey = valueKey;
            }

            public int ValueId { get; private set; }
            public string ValueKey { get; private set; }
        }
    }
}