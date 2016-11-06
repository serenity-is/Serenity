using System;

namespace Serenity.ComponentModel
{
    public class CollapsibleAttribute : Attribute
    {
        public CollapsibleAttribute(bool value = true)
        {
            Value = value;
        }

        public bool Value { get; private set; }
        public bool Collapsed { get; set; }
    }
}