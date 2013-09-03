using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity
{
    public static class EnumMapper
    {
        private static Hashtable cache;

        static EnumMapper()
        {
            cache = new Hashtable();
        }

        private class EnumTypeItem
        {
            public readonly Dictionary<string, object> stringToValue;
            public readonly Dictionary<int, string> valueToString;

            public EnumTypeItem()
            {
                stringToValue = new Dictionary<string,object>(StringComparer.OrdinalIgnoreCase);
                valueToString = new Dictionary<int, string>();
            }
        }

        private static EnumTypeItem Get(Type enumType)
        {
            EnumTypeItem item = cache[enumType] as EnumTypeItem;
            if (item == null)
            {
                item = new EnumTypeItem();

                foreach (var value in Enum.GetValues(enumType))
                {
                    string key = value.ToString();
                    item.stringToValue[key] = value;
                    item.valueToString[Convert.ToInt32(value)] = key;
                }

                var locked = Hashtable.Synchronized(cache);
                locked[enumType] = item;
            }

            return item;
        }

        public static bool TryParse<TEnum>(string key, out TEnum value)
        {
            var item = Get(typeof(TEnum));
            object obj;
            if (item.stringToValue.TryGetValue(key, out obj))
            {
                value = (TEnum)obj;
                return true;
            }
            else
            {
                value = default(TEnum);
                return false;
            }
        }

        public static TEnum Parse<TEnum>(string key)
        {
            var item = Get(typeof(TEnum));
            object obj;
            if (item.stringToValue.TryGetValue(key, out obj))
                return (TEnum)obj;

            throw new ArgumentOutOfRangeException(String.Format("Can't parse {0} enum value {1}!", 
                typeof(TEnum).FullName, key));
        }

        public static string ToString(Type enumType, object value)
        {
            var item = Get(enumType);
            var intValue = Convert.ToInt32(value);
            string key;
            if (item.valueToString.TryGetValue(intValue, out key))
                return key;

            return intValue.ToString();
        }
    }
}