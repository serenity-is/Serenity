using System;

namespace Serenity.ComponentModel
{
    public class DisplayFormatAttribute : Attribute
    {
        public DisplayFormatAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}