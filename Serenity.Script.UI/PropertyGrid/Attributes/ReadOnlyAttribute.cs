using System;

namespace Serenity.ComponentModel
{
    public class ReadOnlyAttribute : Attribute
    {
        public ReadOnlyAttribute(bool readOnly = true)
        {
            this.Value = readOnly;
        }

        public bool Value
        {
            get;
            private set;
        }
    }
}