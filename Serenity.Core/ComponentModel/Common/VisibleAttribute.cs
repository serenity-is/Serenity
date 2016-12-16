using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls visibility of a column / form field.
    /// </summary>
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