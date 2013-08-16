using Serenity.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Basic interface for rows that contains a language ID field.</summary>
    public interface IDbLanguageIdRow: IRow
    {
        /// <summary>
        ///   Gets LanguageID field for row.</summary>
        Int32Field LanguageIdField { get; }
    }
}