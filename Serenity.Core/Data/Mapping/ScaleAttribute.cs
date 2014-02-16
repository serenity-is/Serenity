using System;

namespace Serenity.Data.Mapping
{
    public class ScaleAttribute : Attribute
    {
        public ScaleAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}