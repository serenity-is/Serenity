using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
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
