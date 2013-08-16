using System;

namespace Serenity.ComponentModel
{
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(string defaultValue)
        {
            Value = defaultValue;
        }

        public object Value { get; private set; }
    }
}
