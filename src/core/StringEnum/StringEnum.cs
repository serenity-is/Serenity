using System.Collections.Concurrent;

namespace Serenity;

/// <summary>
/// Base class for string enumerations, which are classes having public constant string fields as their members,
/// an alternative to numeric enums for sets of values that are stored and sent to the client as strings.
/// Only the members declared on the type are part of its set, which is what the server typings generator emits.
/// </summary>
/// <example>
/// <code>
/// [EnumKey("MyApp.WidgetCategory")]
/// public abstract class WidgetCategory : StringEnum
/// {
///     [Description("Charts")]
///     public const string Charts = "charts";
///     public const string Tables = "tables";
/// }
/// </code>
/// </example>
public abstract class StringEnum
{
    private static readonly ConcurrentDictionary<Type, StringEnumInfo> cache = new();

    /// <summary>
    /// Gets whether the given type is a string enumeration type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>True if the type derives from <see cref="StringEnum"/>.</returns>
    public static bool IsStringEnum(Type? type)
    {
        return type != null && type != typeof(StringEnum) && typeof(StringEnum).IsAssignableFrom(type);
    }

    /// <summary>
    /// Gets the cached information for a string enumeration type.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <returns>The information for the type.</returns>
    public static StringEnumInfo GetInfo(Type stringEnumType)
    {
        if (stringEnumType == null)
            throw new ArgumentNullException(nameof(stringEnumType));

        return cache.GetOrAdd(stringEnumType, type => new StringEnumInfo(type));
    }

    /// <summary>
    /// Gets the cached information for a string enumeration type.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <returns>The information for the type.</returns>
    public static StringEnumInfo GetInfo<TStringEnum>() where TStringEnum : StringEnum
    {
        return GetInfo(typeof(TStringEnum));
    }

    /// <summary>
    /// Gets the name of the member with the given value.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <param name="value">The string value.</param>
    /// <returns>The name of the member, or null if the value is not in the set.</returns>
    public static string? GetName(Type stringEnumType, string? value)
    {
        return GetInfo(stringEnumType).FindByValue(value)?.Name;
    }

    /// <summary>
    /// Gets the name of the member with the given value.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <param name="value">The string value.</param>
    /// <returns>The name of the member, or null if the value is not in the set.</returns>
    public static string? GetName<TStringEnum>(string? value) where TStringEnum : StringEnum
    {
        return GetName(typeof(TStringEnum), value);
    }

    /// <summary>
    /// Gets the string value of the member with the given name.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <param name="name">The member name.</param>
    /// <returns>The value of the member, or null if there is no such member.</returns>
    public static string? GetValue(Type stringEnumType, string? name)
    {
        return GetInfo(stringEnumType).FindByName(name)?.Value;
    }

    /// <summary>
    /// Gets the string value of the member with the given name.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <param name="name">The member name.</param>
    /// <returns>The value of the member, or null if there is no such member.</returns>
    public static string? GetValue<TStringEnum>(string? name) where TStringEnum : StringEnum
    {
        return GetValue(typeof(TStringEnum), name);
    }

    /// <summary>
    /// Gets the string values of all the members in their declaration order, including the members hidden from
    /// the client. Like <see cref="Enum.GetValues(Type)"/>, which knows every member of an enum.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <returns>The values of the members.</returns>
    public static IReadOnlyList<string> GetValues(Type stringEnumType)
    {
        return GetInfo(stringEnumType).Values;
    }

    /// <summary>
    /// Gets the string values of all the members in their declaration order, including the members hidden from
    /// the client.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <returns>The values of the members.</returns>
    public static IReadOnlyList<string> GetValues<TStringEnum>() where TStringEnum : StringEnum
    {
        return GetValues(typeof(TStringEnum));
    }

    /// <summary>
    /// Gets the names of all the members in their declaration order, including the members hidden from the client.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <returns>The names of the members.</returns>
    public static IReadOnlyList<string> GetNames(Type stringEnumType)
    {
        return GetInfo(stringEnumType).Names;
    }

    /// <summary>
    /// Gets the names of all the members in their declaration order, including the members hidden from the client.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <returns>The names of the members.</returns>
    public static IReadOnlyList<string> GetNames<TStringEnum>() where TStringEnum : StringEnum
    {
        return GetNames(typeof(TStringEnum));
    }

    /// <summary>
    /// Gets whether the given value is one of the members of the string enumeration.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <param name="value">The string value.</param>
    /// <returns>True if the value is in the set.</returns>
    public static bool IsDefined(Type stringEnumType, string? value)
    {
        return GetInfo(stringEnumType).FindByValue(value) != null;
    }

    /// <summary>
    /// Gets whether the given value is one of the members of the string enumeration.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <param name="value">The string value.</param>
    /// <returns>True if the value is in the set.</returns>
    public static bool IsDefined<TStringEnum>(string? value) where TStringEnum : StringEnum
    {
        return IsDefined(typeof(TStringEnum), value);
    }

    /// <summary>
    /// Gets the display text of the member with the given value, using the <c>Enums.&lt;EnumKey&gt;.&lt;Name&gt;</c>
    /// local text key, like a numeric enum.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <param name="value">The string value.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <returns>The display text of the member, or the value itself if it is not in the set.</returns>
    public static string GetText(Type stringEnumType, string? value, ITextLocalizer? localizer = null)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        var info = GetInfo(stringEnumType);
        var member = info.FindByValue(value);
        if (member == null)
            return value;

        var text = localizer?.TryGet("Enums." + info.EnumKey + "." + member.Name);
        if (text != null)
            return text;

        if (member.TextKey != null)
        {
            var keyedText = localizer?.TryGet(member.TextKey) ?? member.Text;
            if (!string.IsNullOrEmpty(keyedText))
                return keyedText;
        }

        return member.Text ?? member.Name;
    }

    /// <summary>
    /// Gets the display text of the member with the given value, using the <c>Enums.&lt;EnumKey&gt;.&lt;Name&gt;</c>
    /// local text key, like a numeric enum.
    /// </summary>
    /// <typeparam name="TStringEnum">The string enumeration type.</typeparam>
    /// <param name="value">The string value.</param>
    /// <param name="localizer">The text localizer.</param>
    /// <returns>The display text of the member, or the value itself if it is not in the set.</returns>
    public static string GetText<TStringEnum>(string? value, ITextLocalizer? localizer = null) where TStringEnum : StringEnum
    {
        return GetText(typeof(TStringEnum), value, localizer);
    }
}
