using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class IdPropertyAttribute : Attribute
    {
        public IdPropertyAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}