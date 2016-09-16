using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Serenity.Localization;

namespace Serenity
{
    using System.Reflection;
    using Serenity.Abstractions;
    using Serenity.ComponentModel;
    using System.Collections.Concurrent;

    public static class EnumMapper
    {
        private static ConcurrentDictionary<Type, EnumTypeItem> cache;

        static EnumMapper()
        {
            cache = new ConcurrentDictionary<Type, EnumTypeItem>();
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
            EnumTypeItem item;
            if (!cache.TryGetValue(enumType, out item))
            {
                item = new EnumTypeItem();

                foreach (var value in Enum.GetValues(enumType))
                {
                    string key = value.ToString();
                    item.stringToValue[key] = value;
                    item.valueToString[Convert.ToInt32(value)] = key;
                }

                cache[enumType] = item;
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

        public static string GetEnumTypeKey(Type enumType)
        {
            var enumKeyAttr = enumType.GetCustomAttribute<EnumKeyAttribute>();
            return enumKeyAttr != null ? enumKeyAttr.Value : enumType.FullName;
        }

        public static string FormatEnum(Type enumType, object value)
        {
            if (value == null)
                return String.Empty;

            if (enumType != null &&
                enumType.GetIsEnum() &&
                System.Enum.GetName(enumType, value) != null)
            {
                var enumName = System.Enum.GetName(enumType, value);
                var enumKey = GetEnumTypeKey(enumType);
                var key = "Enums." + enumKey + "." + enumName;
                var text = LocalText.TryGet(key);
                if (text == null)
                {
                    var memInfo = enumType.GetMember(enumName);
                    if (memInfo != null && memInfo.Length == 1)
                    {
                        var attribute = memInfo[0].GetCustomAttribute<DescriptionAttribute>(false);
                        if (attribute != null)
                        {
                            text = attribute.Description;
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