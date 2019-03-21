using System;

namespace Serenity.Data
{
    /// <summary>
    /// This attribute marks a row so that when it is Inserted/Updated/Deleted
    /// through repository, its related cache, if any should be cleared. 
    /// It doesn't turn on/off caching. A related cached item to a row, might
    /// be its lookup if any.
    /// </summary>
    public class TwoLevelCachedAttribute : Attribute
    {
        public TwoLevelCachedAttribute(params string[] generationKeys)
        {
            this.GenerationKeys = generationKeys;
        }

        public string[] GenerationKeys { get; private set; }
    }
}