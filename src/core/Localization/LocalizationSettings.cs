namespace Serenity.Localization;

/// <summary>
/// The localization settings of an application, from the "Localization" section of its configuration.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class LocalizationSettings
{
    /// <summary>
    /// The default section key, "Localization".
    /// </summary>
    public const string SectionKey = "Localization";

    /// <summary>
    /// The base language of the application, "en" by default: the language of its invariant texts, which every
    /// language falls back to. The texts of code that is written in another language (see
    /// <see cref="LocalTextLanguageAttribute"/>) are registered for that language instead, and the text files of
    /// the base language, e.g. "texts.tr.json", for the invariant one.
    /// </summary>
    public string? BaseLanguage { get; set; } = "en";

    /// <summary>
    /// The language that a user sees who has not chosen one, and whose browser asks for none of the supported ones.
    /// The base language when not set.
    /// </summary>
    public string? DefaultLanguage { get; set; }
}
