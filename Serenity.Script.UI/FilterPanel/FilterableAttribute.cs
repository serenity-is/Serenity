using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class FilterableAttribute : Attribute
    {
        public FilterableAttribute()
            : this(true)
        {
        }

        public FilterableAttribute(bool value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public bool Value { get; private set; }
    }
}