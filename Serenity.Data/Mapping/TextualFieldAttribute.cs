using System;

namespace Serenity.Data.Mapping
{
    public class TextualFieldAttribute : Attribute
    {
        public TextualFieldAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}