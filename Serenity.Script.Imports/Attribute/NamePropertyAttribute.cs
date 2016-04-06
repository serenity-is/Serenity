using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class NamePropertyAttribute : Attribute
    {
        public NamePropertyAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}