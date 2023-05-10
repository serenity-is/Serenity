
namespace Serenity.Data;

/// <summary>
/// Interface for rows with a CultureId field
/// </summary>
public interface ILocalizationRow : IIdRow
{
    /// <summary>
    /// Culture ID field, e.g. LanguageID / LanguageKey
    /// </summary>
    Field CultureIdField { get; }
}