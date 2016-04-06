using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class LocalTextPrefixAttribute : Attribute
    {
        public LocalTextPrefixAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}