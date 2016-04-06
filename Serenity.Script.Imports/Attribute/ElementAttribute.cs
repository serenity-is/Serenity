using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class ElementAttribute : Attribute
    {
        public ElementAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value
        {
            get; private set;
        }
    }
}