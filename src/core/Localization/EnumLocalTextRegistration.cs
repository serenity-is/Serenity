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
                    if (descAttr != null)
                        registry.Add(languageID, "Enums." + enumKey + "." + name, descAttr.Description);
                }
            }
        }
    }
}