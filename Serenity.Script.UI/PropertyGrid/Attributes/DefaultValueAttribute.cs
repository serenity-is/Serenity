using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(string defaultValue)
        {
            Value = defaultValue;
        }

        [IntrinsicProperty]
        public object Value { get; private set; }
    }
}
