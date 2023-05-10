namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for local text registry that can return language fallbacks
/// </summary>
public interface ILanguageFallbacks
{
    /// <summary>
    /// Gets the explicitly registered language fallbacks
    /// </summary>
    IDictionary<string, string> GetLanguageFallbacks();

    /// <summary>
    /// Sets the language fallback of the specified language.
    /// When a text is not found in one language, LocalTextRegistry checks its language fallback for
    /// a translation. Some implicit language fallback definitions exist even if none set. For example, "en" is 
    /// language fallback ID of "en-US" and "en-UK", "tr" is language fallback ID of "tr-TR". Also, 
    /// invariant language ID ("") is an implicit fallback of all languages.
    /// </summary>
    /// <param name="languageID">Language identifier. (e.g. en-US)</param>
    /// <param name="fallbackID">language fallback identifier. (e.g. en)</param>
    void SetLanguageFallback(string languageID, string fallbackID);
}