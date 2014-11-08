using System;

namespace Serenity.ComponentModel
{
    public class NotFilterableAttribute : Attribute
    {
        public NotFilterableAttribute(bool value = true)
        {
            this.Value = value;
        }

        public bool Value
        {
            get;
            private set;
        }
    }
}
