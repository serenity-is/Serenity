using System;

namespace Serenity.ComponentModel
{
    public class VisibleAttribute : Attribute
    {
        public VisibleAttribute(bool value = true)
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