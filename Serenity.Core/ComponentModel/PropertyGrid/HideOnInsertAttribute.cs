using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls whether this field is visible on new record mode
    /// </summary>
    public class HideOnInsertAttribute : Attribute
    {
        /// <summary>
        /// Controls whether this field is visible on new record mode
        /// </summary>
        /// <param name="value">True to hide field on insert</param>
        public HideOnInsertAttribute(bool value = true)
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