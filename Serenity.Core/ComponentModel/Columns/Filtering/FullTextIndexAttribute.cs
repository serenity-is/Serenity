using System;

namespace Serenity.ComponentModel
{
    public class FullTextIndexAttribute : Attribute
    {
        public FullTextIndexAttribute(bool value = true)
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
