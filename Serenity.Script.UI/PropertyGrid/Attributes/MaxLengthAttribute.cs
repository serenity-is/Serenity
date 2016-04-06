using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class MaxLengthAttribute : Attribute
    {
        public MaxLengthAttribute(int maxLength)
        {
            this.MaxLength = maxLength;
        }

        [IntrinsicProperty]
        public int MaxLength { get; private set; }
    }
}
