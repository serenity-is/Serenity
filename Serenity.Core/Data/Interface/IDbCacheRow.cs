using Serenity.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Basic interface for rows that contains a DbSharedLookupCache.</summary>
    public interface IDbCacheRow
    {
        /// <summary>
        ///   Gets lookup cache object for this row.</summary>
        IDbLookupCache Cache { get; }
    }
}