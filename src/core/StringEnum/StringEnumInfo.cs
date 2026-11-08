using System.Collections.ObjectModel;
using System.Reflection;

namespace Serenity;

/// <summary>
/// The members and lookups of a <see cref="StringEnum"/> type, which are built once for the type and cached
/// by <see cref="StringEnum.GetInfo(Type)"/>.
/// </summary>
/// <remarks>
/// Only the members declared on the type itself are read, like the server typings generator, so a type deriving
/// from another one has its own set. When two members have the same value, the one declared first is used,
/// like the first icon of an alias in a numeric enum. All members are included, also the ones hidden from the
/// client with <see cref="IgnoreUIFieldAttribute"/> or <see cref="TransformIgnoreAttribute"/>, as the server
/// accepts them; <see cref="StringEnumMember.IsVisible"/> tells which ones are sent to the client.
/// </remarks>
public sealed class StringEnumInfo
{
    private readonly Dictionary<string, StringEnumMember> byValue;
    private readonly Dictionary<string, StringEnumMember> byName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEnumInfo"/> class.
    /// </summary>
    /// <param name="stringEnumType">The string enumeration type.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stringEnumType"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="stringEnumType"/> is not a string enumeration type.</exception>
    internal StringEnumInfo(Type stringEnumType)
    {
        if (stringEnumType == null)
            throw new ArgumentNullException(nameof(stringEnumType));

        if (!StringEnum.IsStringEnum(stringEnumType))
            throw new ArgumentOutOfRangeException(nameof(stringEnumType),
                $"{stringEnumType.FullName} is not a {nameof(StringEnum)} type!");

        Type = stringEnumType;
        EnumKey = EnumMapper.GetEnumTypeKey(stringEnumType);

        // both lookups are built here, so a value is never missed while a table is being filled on first use
        byValue = new Dictionary<string, StringEnumMember>(StringComparer.Ordinal);
        byName = new Dictionary<string, StringEnumMember>(StringComparer.Ordinal);

        var members = new List<StringEnumMember>();
        foreach (var field in stringEnumType.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.IsLiteral && x.FieldType == typeof(string))
            .OrderBy(x => x.MetadataToken))
        {
            if (field.GetRawConstantValue() is not string value)
                continue;

            var hidden = field.GetCustomAttribute<IgnoreUIFieldAttribute>() != null ||
                field.GetCustomAttribute<TransformIgnoreAttribute>() != null;

            var textAttr = (Attribute?)field.GetCustomAttribute<DisplayTextAttribute>() ??
                field.GetCustomAttribute<DescriptionAttribute>();
            var text = textAttr switch
            {
                DisplayTextAttribute displayText => displayText.DisplayName,
                DescriptionAttribute description => description.Description,
                _ => null
            };
            var resolved = textAttr != null ? LocalText.ResolveFromAttribute(textAttr, text, defaultKey: null) : null;

            var member = new StringEnumMember(field.Name, value, resolved?.Key, resolved?.Text, isVisible: !hidden);
            members.Add(member);

            // the first member with a value wins, as in the icons of aliases in a numeric enum
            if (!byValue.ContainsKey(value))
                byValue[value] = member;
            byName[member.Name] = member;
        }

        Members = new ReadOnlyCollection<StringEnumMember>(members);
        VisibleMembers = new ReadOnlyCollection<StringEnumMember>(members.Where(x => x.IsVisible).ToArray());
        Names = new ReadOnlyCollection<string>(members.Select(x => x.Name).ToArray());
        Values = new ReadOnlyCollection<string>(members.Select(x => x.Value).ToArray());
    }

    /// <summary>
    /// Gets the string enumeration type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the key of the string enumeration, which is its <see cref="EnumKeyAttribute"/> value, or its full name.
    /// </summary>
    public string EnumKey { get; }

    /// <summary>
    /// Gets all the members in their declaration order, including the ones hidden from the client.
    /// </summary>
    public IReadOnlyList<StringEnumMember> Members { get; }

    /// <summary>
    /// Gets the members that are sent to the client, in their declaration order.
    /// </summary>
    public IReadOnlyList<StringEnumMember> VisibleMembers { get; }

    /// <summary>
    /// Gets the names of all the members in their declaration order.
    /// </summary>
    public IReadOnlyList<string> Names { get; }

    /// <summary>
    /// Gets the values of all the members in their declaration order.
    /// </summary>
    public IReadOnlyList<string> Values { get; }

    /// <summary>
    /// Finds the member with the given value.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The member with the value, or null if none.</returns>
    public StringEnumMember? FindByValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        return byValue.TryGetValue(value, out var found) ? found : null;
    }

    /// <summary>
    /// Finds the member with the given name.
    /// </summary>
    /// <param name="name">The member name.</param>
    /// <returns>The member with the name, or null if none.</returns>
    public StringEnumMember? FindByName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return byName.TryGetValue(name, out var found) ? found : null;
    }
}
