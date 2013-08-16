using Serenity.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Interface for lookup rows that has an ID field, name field, a DbSharedLookupCache, 
    ///   and a display order field.</summary>
    public interface IDbLookupRow: IIdRow, IDbCacheRow, INameRow
    {
    }
}