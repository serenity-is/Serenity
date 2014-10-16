using System;

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

        public bool Value { get; private set; }
    }
}