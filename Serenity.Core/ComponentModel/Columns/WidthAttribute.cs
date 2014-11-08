using System;

namespace Serenity.ComponentModel
{
    public class WidthAttribute : Attribute
    {
        public WidthAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        public int Min { get; set; }
        public int Max { get; set; }
    }
}