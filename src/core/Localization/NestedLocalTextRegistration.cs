namespace Serenity.Localization;

/// <summary>
/// Contains helper methods for registration of local texts in nested static classes.
/// Nested static classes contain <see cref="LocalText"/> objects with actual translations as keys. This class locates
/// them (with <see cref="NestedLocalTextsAttribute"/> at the outermost class), determines keys by the path from the outermost
/// to the nested class name, replaces the existing <see cref="LocalText"/> instance with an initialized instance
/// containing this generated key and initial translation, and registers this translation in the
/// <see cref="ILocalTextRegistry"/> provider.
/// </summary>
public static class NestedLocalTextRegistration
{
    /// <summary>
    /// Adds translations from static nested local text classes marked with <see cref="NestedLocalTextsAttribute"/>.
    /// </summary>
    /// <param name="registry">The registry to add texts to.</param>
    /// <param name="typeSource">The type source to search for nested local text classes in.</param>
    public static void AddNestedTexts(this ILocalTextRegistry registry, ITypeSource typeSource)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        foreach (var type in typeSource.GetTypesWithAttribute(typeof(NestedLocalTextsAttribute)))
        {
            var attr = type.GetCustomAttribute<NestedLocalTextsAttribute>();
            if (attr != null)
            {
                Initialize(registry, type, attr.LanguageID ?? LocalText.InvariantLanguageID, 
                    attr.Prefix ?? "");
            }
        }
    }

    private static void Initialize(ILocalTextRegistry registry, Type type, 
        string languageID, string prefix)
    {
        if (registry == null)
            throw new ArgumentNullException(nameof(registry));

        var provider = registry;
        foreach (var member in type.GetMembers(
            BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var fi = member as FieldInfo;
            if (fi != null &&
                fi.FieldType == typeof(LocalText) &&
                fi.GetValue(null) is ILocalText value)
            { 
                if (value.OriginalKey is null)
                    value.ReplaceKey(prefix + fi.Name);

                provider.Add(languageID, value.Key, value.OriginalKey);
            }
        }

        foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var name = nested.Name;
            if (name.EndsWith("_"))
                name = name[0..^1];

            Initialize(registry, nested, languageID, prefix + name + ".");
        }
    }
}