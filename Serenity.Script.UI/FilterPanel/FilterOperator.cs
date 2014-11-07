using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class FilterOperator
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }

        [InlineCode("{{key:{key}}}")]
        public static implicit operator FilterOperator(string key)
        {
            return null;
        }
    }
}
