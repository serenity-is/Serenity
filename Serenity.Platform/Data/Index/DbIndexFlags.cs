using System;

namespace Serenity.Data
{
    /// <summary>
    ///   Flags that controls DbIndex sorting.</summary>
    [Flags]
    public enum DbIndexFlags
    {
        /// <summary>No special flags (default).</summary>
        None = 0,
        /// <summary>İndeks must be unique (unused for now).</summary>
        Unique = 1,
        /// <summary>Place nulls at tail (interbase).</summary>
        PlaceNullsAtTail = 2,
        /// <summary>Keep nulls at same position even for descending sorts (interbase).</summary>
        NoDescendingNulls = 4
    }
}
