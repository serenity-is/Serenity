namespace Serenity.Localization;

/// <summary>
/// A text registry that knows the base language of the application, i.e. the language of its invariant texts. The
/// texts added for the base language are stored as invariant ones, and the texts of the code that is written in
/// another language (see <see cref="LocalTextLanguageAttribute"/>) are looked up in that language when the invariant
/// language has none, e.g. an English text of a feature that the Turkish base language has no translation of.
/// </summary>
public interface IBaseLanguage
{
    /// <summary>
    /// The base language, e.g. "tr", or null when it is not set, which is taken as English, the language of the
    /// texts of Serenity, without the English texts being stored as invariant ones.
    /// </summary>
    string? BaseLanguageID { get; set; }

    /// <summary>
    /// Adds a language that texts of the code are written in besides the base language, which a text that the
    /// invariant language has none of is looked up in.
    /// </summary>
    /// <param name="languageID">The language ID, e.g. "en".</param>
    void AddSourceLanguage(string languageID);

    /// <summary>
    /// The languages that texts of the code are written in besides the base language.
    /// </summary>
    IReadOnlyList<string> SourceLanguages { get; }
}
