namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for a local text registry which stores translations for local text keys.
/// </summary>
public interface ILocalTextRegistry
{
    /// <summary>
    /// Returns the localized representation that corresponds to the local text key,
    /// or <c>null</c> if none is found in the source.
    /// </summary>
    /// <param name="key">The local text key (e.g. Enums.Month.June).</param>
    /// <param name="languageID">The language identifier.</param>
    /// <param name="pending">Include pending (not approved) texts.</param>
    /// <returns>The localized text, or <c>null</c> if none is found.</returns>
    string? TryGet(string languageID, string key, bool pending);

    /// <summary>
    /// Adds a local text entry to the registry.
    /// </summary>
    /// <param name="languageID">The language ID (e.g. en-US, tr-TR).</param>
    /// <param name="key">The local text key.</param>
    /// <param name="text">The translated text.</param>
    void Add(string languageID, string key, string? text);
}