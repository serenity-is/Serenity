namespace Serenity.Abstractions;

/// <summary>
/// Provides local text keys the application stores in its data, e.g. the keys that dashboards use for their titles, so
/// that the translation page lists them for the translators, which it does only for the keys that are registered by
/// the code and texts files otherwise. It only tells which keys there are and what text they are meant to be translated
/// from, while the translated texts are in the local text registry.
/// </summary>
public interface ITranslationKeyProvider
{
    /// <summary>
    /// Gets the keys with the texts they are meant to be translated from, which are what is shown for the key
    /// when it has no text in the registry.
    /// </summary>
    /// <returns>The keys with their source texts, which can be null when there is none.</returns>
    IEnumerable<KeyValuePair<string, string?>> GetKeys();
}
