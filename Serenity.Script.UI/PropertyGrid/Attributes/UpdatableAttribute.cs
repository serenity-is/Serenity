using System;

namespace Serenity.ComponentModel
{
    public class UpdatableAttribute : Attribute
    {
        public UpdatableAttribute(bool updatable = true)
        {
            this.Value = updatable;
        }

        public bool Value
        {
            get;
            private set;
        }
    }
}