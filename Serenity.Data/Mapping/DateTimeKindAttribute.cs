using System;

namespace Serenity.Data
{
    public class DateTimeKindAttribute : Attribute
    {
        public DateTimeKindAttribute(DateTimeKind value)
        {
            this.Value = value;
        }

        public DateTimeKind Value { get; private set; }
    }
}