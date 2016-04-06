using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class ItemNameAttribute : Attribute
    {
        public ItemNameAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}