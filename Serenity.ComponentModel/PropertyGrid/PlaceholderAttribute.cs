using System;

namespace Serenity.ComponentModel
{
    public class PlaceholderAttribute : Attribute
    {
        public PlaceholderAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
