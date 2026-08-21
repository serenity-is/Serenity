namespace Serenity.Localization;

/// <summary>
/// A pair of a language ID and a text key.
/// </summary>
/// <param name="LanguageId">The language ID.</param>
/// <param name="Key">The local text key.</param>
public record struct LanguageIdKeyPair(string LanguageId, string Key);
