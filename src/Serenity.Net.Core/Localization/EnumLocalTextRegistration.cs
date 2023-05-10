namespace Serenity.Localization;

/// <summary>
/// Contains initialization method for adding local text translations defined by
/// Description attributes in enumeration classes.
/// </summary>
public static class EnumLocalTextRegistration
{
    /// <summary>
    /// Adds local text translations defined implicitly by Description attributes in 
    /// enumeration classes. Only enum values that has Description attribute are added as
    /// local text. By default, enums are registered in format:
    /// "Enums.{EnumerationTypeFullName}.{EnumValueName}". EnumerationTypeFullName, is
    /// fullname of the enumeration type. This can be overridden by attaching a EnumKey
    /// attribute.
    /// </summary>
    /// <param name="typeSource">Type source to search for enumeration classes in</param>
    /// <param name="languageID">Language ID texts will be added (default is invariant language)</param>
    /// <param name="registry">Registry</param>
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