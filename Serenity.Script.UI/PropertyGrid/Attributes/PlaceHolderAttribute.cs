using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class PlaceholderAttribute : Attribute
    {
        public PlaceholderAttribute(string value)
        {
            Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}
