using System;

namespace Serenity.ComponentModel
{
    public class FilteringIdFieldAttribute : Attribute
    {
        public FilteringIdFieldAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
