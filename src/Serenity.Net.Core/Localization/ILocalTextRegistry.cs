namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for local text registry which stores translations for local text keys. 
/// </summary>
public interface ILocalTextRegistry
{
    /// <summary>
    /// Returns localized representation which corresponds to the local text key 
    /// or null if none found in the source.
    /// </summary>
    /// <param name="key">Local text key (e.g. Enums.Month.June)</param>
    /// <param name="languageID">Language identifier</param>
    /// <param name="pending">Include pending (not approved) texts</param>
    /// <returns></returns>
    string? TryGet(string languageID, string key, bool pending);

    /// <summary>
    /// Adds a local text entry to the registry
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    void Add(string languageID, string key, string? text);
}