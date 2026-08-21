using Serenity.Localization;

namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for a local text registry that can return all registered entries.
/// </summary>
public interface IGetAllTexts
{
    /// <summary>
    /// Gets all local text entries.
    /// </summary>
    /// <param name="pending"><c>true</c> to return pending (not yet approved) texts.</param>
    /// <returns>A dictionary of all local text entries.</returns>
    IDictionary<LanguageIdKeyPair, string?> GetAllTexts(bool pending);
}