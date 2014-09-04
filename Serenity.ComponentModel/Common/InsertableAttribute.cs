using System;

namespace Serenity.ComponentModel
{
    public class InsertableAttribute : Attribute
    {
        public InsertableAttribute(bool insertable = true)
        {
            this.Value = insertable;
        }

        public bool Value
        {
            get;
            private set;
        }
    }
}