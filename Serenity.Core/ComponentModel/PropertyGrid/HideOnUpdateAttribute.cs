using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls whether this field is visible on edit record mode
    /// </summary>
    public class HideOnUpdateAttribute : Attribute
    {
        /// <summary>
        /// Controls whether this field is visible on edit record mode
        /// </summary>
        /// <param name="value">True to hide field on update</param>
        public HideOnUpdateAttribute(bool value = true)
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