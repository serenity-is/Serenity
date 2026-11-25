namespace Serenity.Localization;

/// <summary>
/// Works out the language that the texts of the code of an assembly are registered for, see
/// <see cref="LocalTextLanguageAttribute"/> and <see cref="IBaseLanguage"/>.
/// </summary>
public static class LocalTextLanguages
{
    /// <summary>
    /// The language that the texts of Serenity are written in, and the base language when none is set.
    /// </summary>
    public const string DefaultBaseLanguage = "en";

    /// <summary>
    /// Gets whether two languages are the same as far as the texts are concerned, e.g. "en" and "en-US": a language
    /// is the same as a regional one of it.
    /// </summary>
    /// <param name="languageID">A language ID.</param>
    /// <param name="otherID">Another language ID.</param>
    public static bool IsSameLanguage(string? languageID, string? otherID)
    {
        if (string.IsNullOrEmpty(languageID) || string.IsNullOrEmpty(otherID))
            return string.IsNullOrEmpty(languageID) && string.IsNullOrEmpty(otherID);

        if (string.Equals(languageID, otherID, StringComparison.OrdinalIgnoreCase))
            return true;

        static bool isRegionOf(string region, string language) => region.Length > language.Length &&
            region[language.Length] == '-' && region.StartsWith(language, StringComparison.OrdinalIgnoreCase);

        return isRegionOf(languageID, otherID) || isRegionOf(otherID, languageID);
    }

    /// <summary>
    /// Gets the language that the texts of the code of an assembly are registered for in a registry: the invariant
    /// language when the assembly has no <see cref="LocalTextLanguageAttribute"/>, or its language is the base one,
    /// else its language, which is then added to the source languages of the registry.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="languageID">The language that the caller asked for; the one of the assembly is used only when
    /// this is the invariant language.</param>
    public static string ForAssembly(ILocalTextRegistry registry, Assembly? assembly, string languageID = LocalText.InvariantLanguageID)
    {
        if (!string.IsNullOrEmpty(languageID) || assembly is null)
            return languageID ?? LocalText.InvariantLanguageID;

        var language = assembly.GetCustomAttribute<LocalTextLanguageAttribute>()?.LanguageID;
        if (string.IsNullOrEmpty(language))
            return LocalText.InvariantLanguageID;

        var baseLanguage = registry as IBaseLanguage;
        if (IsSameLanguage(language, baseLanguage?.BaseLanguageID ?? DefaultBaseLanguage))
            return LocalText.InvariantLanguageID;

        baseLanguage?.AddSourceLanguage(language);
        return language;
    }

    /// <summary>
    /// Gets the language that the texts of the code of a type are registered for in a registry, see
    /// <see cref="ForAssembly"/>.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="type">The type.</param>
    /// <param name="languageID">The language that the caller asked for; the one of the assembly of the type is used
    /// only when this is the invariant language.</param>
    public static string ForType(ILocalTextRegistry registry, Type? type, string languageID = LocalText.InvariantLanguageID)
    {
        return ForAssembly(registry, type?.Assembly, languageID);
    }
}
