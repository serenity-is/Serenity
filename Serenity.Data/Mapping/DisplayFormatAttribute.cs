using System;

namespace Serenity.Data.Mapping
{
    public class DisplayFormatAttribute : Attribute
    {
        public DisplayFormatAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}