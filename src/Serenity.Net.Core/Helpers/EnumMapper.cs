namespace Serenity;

/// <summary>
/// Contains Enum mapping and other helper functions
/// </summary>
public static class EnumMapper
{
    private static readonly ConcurrentDictionary<Type, EnumTypeItem> cache;

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
            stringToValue = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            valueToString = new Dictionary<int, string>();
        }
    }

    private static EnumTypeItem Get(Type enumType)
    {
        if (!cache.TryGetValue(enumType, out EnumTypeItem item))
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

    /// <summary>
    /// Tries to parse the enum string.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="key">The enumeration key or integer value.</param>
    /// <param name="value">The value.</param>
    /// <returns>If parsed successfully true</returns>
    public static bool TryParse<TEnum>(string key, out TEnum value)
    {
        var item = Get(typeof(TEnum));
        if (item.stringToValue.TryGetValue(key, out object obj))
        {
            value = (TEnum)obj;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    /// <summary>
    /// Parses the specified enum key.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="key">The enum key or numeric value.</param>
    /// <returns>Parsed enum value</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enum value is not valid.</exception>
    public static TEnum Parse<TEnum>(string key)
    {
        var item = Get(typeof(TEnum));
        if (item.stringToValue.TryGetValue(key, out object obj))
            return (TEnum)obj;

        throw new ArgumentOutOfRangeException(string.Format("Can't parse {0} enum value {1}!",
            typeof(TEnum).FullName, key));
    }

    /// <summary>
    /// Converts an enum value to string.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="string" /> that represents enum value.
    /// This is the identifier of the enum value or a numeric value.
    /// </returns>
    public static string ToString(Type enumType, object value)
    {
        var item = Get(enumType);
        var intValue = Convert.ToInt32(value);
        if (item.valueToString.TryGetValue(intValue, out string key))
            return key;

        return intValue.ToString();
    }

    /// <summary>
    /// Gets the name of the enum value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Enum name.</returns>
    public static string GetName(this Enum value)
    {
        if (value == null)
            return string.Empty;

        return Enum.GetName(value.GetType(), value);
    }

    /// <summary>
    /// Gets the display text of the enum value.
    /// </summary>
    /// <param name="localizer">Text localizer</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string GetText(this Enum value, ITextLocalizer localizer)
    {
        if (value == null)
            return string.Empty;

        return FormatEnum(localizer, value.GetType(), value);
    }

    /// <summary>
    /// Gets the enum type key.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <returns>Enum type key</returns>
    public static string GetEnumTypeKey(Type enumType)
    {
        var enumKeyAttr = enumType.GetCustomAttribute<EnumKeyAttribute>();
        return enumKeyAttr != null ? enumKeyAttr.Value : enumType.FullName;
    }

    /// <summary>
    /// Formats the enum.
    /// </summary>
    /// <param name="localizer">Text localizer</param>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string FormatEnum(this ITextLocalizer localizer, Type enumType, object value)
    {
        if (value == null)
            return string.Empty;

        if (enumType != null &&
            enumType.IsEnum &&
            System.Enum.GetName(enumType, value) != null)
        {
            var enumName = System.Enum.GetName(enumType, value);
            var enumKey = GetEnumTypeKey(enumType);
            var key = "Enums." + enumKey + "." + enumName;
            var text = localizer?.TryGet(key);
            if (text == null)
            {
                var memInfo = enumType.GetMember(enumName);
                if (memInfo != null && memInfo.Length == 1)
                {
                    var attribute = memInfo[0].GetCustomAttribute<DescriptionAttribute>(false);
                    if (attribute != null)
                    {
                        text = attribute.Description;
                    }
                }
            }

            return text ?? enumName;
        }
        else
            return value.ToString();
    }
}