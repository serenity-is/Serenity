using System;

namespace Serenity.Data.Mapping
{
    public class SizeAttribute : Attribute
    {
        public SizeAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}