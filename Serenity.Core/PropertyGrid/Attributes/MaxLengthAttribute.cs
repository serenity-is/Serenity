using System;

namespace Serenity.ComponentModel
{
    public class MaxLengthAttribute : Attribute
    {
        public MaxLengthAttribute(int maxLength)
        {
            this.MaxLength = maxLength;
        }

        public int MaxLength { get; private set; }
    }
}
