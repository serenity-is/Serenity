using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class FullTextIndexAttribute : Attribute
    {
        public FullTextIndexAttribute() : this(true)
        {
        }

        public FullTextIndexAttribute(bool value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public bool Value { get; private set; }
    }
}