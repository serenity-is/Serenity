using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Serenity.Localization;

namespace Serenity
{
    using System.Reflection;
    using Serenity.ComponentModel;

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

        public static string GetName(this Enum value)
        {
            return System.Enum.GetName(value.GetType(), value);
        }

        public static string GetText(this Enum value)
        {
            return FormatEnum(value.GetType(), value);
        }

        public static string FormatEnum(Type enumType, object value)
        {
            if (value == null)
                return String.Empty;

            if (enumType != null &&
                enumType.IsEnum &&
                System.Enum.GetName(enumType, value) != null)
            {
                var enumKeyAttr = enumType.GetCustomAttribute<EnumKeyAttribute>();
                var enumKey = enumKeyAttr != null ? enumKeyAttr.Value : enumType.FullName;

                var enumName = System.Enum.GetName(enumType, value);
                var key = "Enums." + enumKey + "." + enumName;
                var text = LocalText.TryGet(key);
                if (text == null)
                {
                    var memInfo = enumType.GetMember(enumName);
                    if (memInfo != null && memInfo.Length == 1)
                    {
                        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attributes.Length > 0)
                        {
                            text = ((DescriptionAttribute)attributes[0]).Description;
                            Dependency.Resolve<ILocalTextRegistry>().Add(LocalText.InvariantLanguageID, key, text);
                        }
                    }
                }

                return text ?? enumName;
            }
            else
                return value.ToString();
        }
    }
}