namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for a local text registry that can return language fallbacks.
/// </summary>
public interface ILanguageFallbacks
{
    /// <summary>
    /// Gets the language fallbacks for the specified language ID. It returns an empty list
    /// for the invariant language. For other language IDs, the last element is always
    /// the invariant language ID.
    /// </summary>
    /// <param name="languageID">The language ID.</param>
    /// <returns>The sequence of language fallback IDs.</returns>
    IEnumerable<string> GetLanguageFallbacks(string languageID);

    /// <summary>
    /// Sets the language fallback of the specified language.
    /// When a text is not found in one language, the local text registry checks its language fallback for
    /// a translation. Some implicit language fallback definitions exist even if none are set. For example, "en" is
    /// the language fallback ID of "en-US" and "en-UK", and "tr" is the language fallback ID of "tr-TR". Also,
    /// the invariant language ID ("") is an implicit fallback of all languages.
    /// </summary>
    /// <param name="languageID">The language identifier (e.g. en-US).</param>
    /// <param name="fallbackID">The language fallback identifier (e.g. en).</param>
    void SetLanguageFallback(string languageID, string fallbackID);
}