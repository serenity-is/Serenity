namespace Serenity.ComponentModel;

/// <summary>
/// Tells the language that the texts in the code of an assembly are written in, e.g. its display names, nested local
/// texts and enum descriptions. Those texts are registered for the invariant language when it is the base language of
/// the application (see <see cref="Localization.LocalizationSettings.BaseLanguage"/>), and for this language
/// otherwise. The texts of an assembly without this attribute are taken to be in the base language.
/// </summary>
/// <param name="languageID">The language ID, e.g. "en".</param>
/// <example><code>[assembly: LocalTextLanguage("en")]</code></example>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class LocalTextLanguageAttribute(string languageID) : Attribute
{
    /// <summary>
    /// The language ID, e.g. "en".
    /// </summary>
    public string LanguageID { get; } = languageID ?? throw new ArgumentNullException(nameof(languageID));
}
