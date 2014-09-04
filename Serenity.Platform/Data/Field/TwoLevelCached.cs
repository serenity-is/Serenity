using System;

namespace Serenity.Data
{
    public class TwoLevelCachedAttribute : Attribute
    {
        public TwoLevelCachedAttribute(params string[] generationKeys)
        {
            this.GenerationKeys = generationKeys;
        }

        public string[] GenerationKeys { get; private set; }
    }
}