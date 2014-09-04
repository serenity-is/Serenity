using System;

namespace Serenity.Data.Mapping
{
    public class ExpressionAttribute : Attribute
    {
        public ExpressionAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}