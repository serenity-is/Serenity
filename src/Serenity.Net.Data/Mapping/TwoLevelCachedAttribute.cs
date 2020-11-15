using System;

namespace Serenity.Data
{
    /// <summary>
    /// This attribute marks a row so that when it is Inserted/Updated/Deleted
    /// through repository, its related cache, if any should be cleared. 
    /// It doesn't turn on/off caching. A sample of related cached item to a row, might
    /// be its lookup if any.
    /// </summary>
    public class TwoLevelCachedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwoLevelCachedAttribute"/> class.
        /// </summary>
        /// <param name="generationKeys">The generation keys.</param>
        public TwoLevelCachedAttribute(params string[] generationKeys)
        {
            this.GenerationKeys = generationKeys;
        }

        /// <summary>
        /// Gets the generation keys.
        /// </summary>
        /// <value>
        /// The generation keys.
        /// </value>
        public string[] GenerationKeys { get; private set; }
    }
}