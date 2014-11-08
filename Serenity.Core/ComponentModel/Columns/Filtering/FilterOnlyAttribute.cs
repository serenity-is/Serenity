using System;

namespace Serenity.ComponentModel
{
    public class FilterOnlyAttribute : Attribute
    {
        public FilterOnlyAttribute(bool value = true)
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
