using System;

namespace Serenity.ComponentModel
{
    public class SortableAttribute : Attribute
    {
        public SortableAttribute(bool value = true)
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
