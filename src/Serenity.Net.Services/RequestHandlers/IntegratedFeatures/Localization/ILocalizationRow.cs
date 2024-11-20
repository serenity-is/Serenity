
namespace Serenity.Data;

/// <summary>
/// Interface for rows with a CultureId field
/// </summary>
public interface ILocalizationRow : IIdRow
{
    /// <summary>
    /// Language ID field (must be a two or four letter culture code, e.g. en-GB)
    /// </summary>
    StringField CultureIdField { get; }
}