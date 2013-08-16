using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Bir ID değerine karşılık gelen metni sunan arayüz.</summary>
    public interface IDbLookupCache : IDbCache, IEnumerable, IEnumerable<IDbLookupRow>
    {
        /// <summary>
        ///   Returns a name by ID.</summary>
        /// <param name="id">
        ///   ID.</param>
        /// <returns>
        ///   Name for given ID.</returns>
        string GetNameByID(Int64 id);
        /// <summary>
        ///   Returns a name by ID.</summary>
        /// <param name="id">
        ///   ID.</param>
        /// <returns>
        ///   Name for given ID.</returns>
        Int64? GetIDByName(string name);
    }
}