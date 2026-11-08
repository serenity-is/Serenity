using System.ComponentModel;

namespace Serenity.Localization;

/// <summary>
/// Contains initialization methods for adding local text translations defined by
/// <see cref="DescriptionAttribute"/> attributes in enumeration classes.
/// </summary>
public static class EnumLocalTextRegistration
{
    /// <summary>
    /// Adds local text translations defined implicitly by <see cref="DescriptionAttribute"/> attributes in
    /// enumeration classes. Only enum values that have a <see cref="DescriptionAttribute"/> are added as
    /// local text. By default, enums are registered in the format
    /// "Enums.{EnumerationTypeFullName}.{EnumValueName}", where EnumerationTypeFullName is the
    /// full name of the enumeration type. This can be overridden by attaching an <see cref="EnumKeyAttribute"/>.
    /// <para>
    /// The members of <see cref="StringEnum"/> types are registered with the same keys, so a string enumeration
    /// can't share a member name with an enum that has the same key.
    /// </para>
    /// </summary>
    /// <param name="registry">The registry to add texts to.</param>
    /// <param name="typeSource">The type source to search for enumeration classes in.</param>
    /// <param name="languageID">The language ID texts will be added for (default is the invariant language).</param>
    public static void AddEnumTexts(this ILocalTextRegistry registry, ITypeSource typeSource,
        string languageID = LocalText.InvariantLanguageID)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        var owners = new Dictionary<string, Type>(StringComparer.Ordinal);

        foreach (var type in typeSource.GetTypes())
        {
            if (type.IsEnum)
            {
                var enumKey = EnumMapper.GetEnumTypeKey(type);

                foreach (var name in Enum.GetNames(type))
                {
                    var member = type.GetMember(name);
                    if (member.Length == 0)
                        continue;

                    var descAttr = member[0].GetCustomAttribute<DescriptionAttribute>();
                    if (descAttr == null)
                        continue;

                    var defaultKey = "Enums." + enumKey + "." + name;
                    var resolved = LocalText.ResolveFromAttribute(descAttr, descAttr.Description, defaultKey);
                    if (resolved.Key is string key)
                        AddText(registry, owners, type, key, resolved.Text, languageID);
                }
            }
            else if (StringEnum.IsStringEnum(type))
            {
                var info = StringEnum.GetInfo(type);

                foreach (var member in info.Members)
                {
                    if (member.TextKey is null && member.Text is null)
                        continue;

                    var key = member.TextKey ?? "Enums." + info.EnumKey + "." + member.Name;
                    AddText(registry, owners, type, key, member.Text, languageID);
                }
            }
        }
    }

    private static void AddText(ILocalTextRegistry registry, Dictionary<string, Type> owners,
        Type type, string key, string? text, string languageID)
    {
        if (owners.TryGetValue(key, out var ownerType) && ownerType != type)
            throw new InvalidOperationException(string.Format(
                "The local text key '{0}' of {1} is already used by {2}. " +
                "Please change the EnumKey of one of them, or the name of its members!",
                key, type.FullName, ownerType.FullName));

        owners[key] = type;

        registry.Add(languageID, key, text);
    }
}
