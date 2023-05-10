using Serenity.Localization;

namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for local text registry that can return all registered entries
/// </summary>
public interface IGetAllTexts
{
    /// <summary>
    /// Gets all local text entries
    /// </summary>
    IDictionary<LanguageIdKeyPair, string?> GetAllTexts(bool pending);
}