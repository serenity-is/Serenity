using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls if this field is editable in update record mode.
    /// When used with fields, turns on or off the updatable flag.
    /// </summary>
    public class UpdatableAttribute : Attribute
    {
        /// <summary>
        /// Controls if this field is editable in update record mode.
        /// When used with fields, turns on or off the updatable flag.
        /// </summary>
        /// <param name="updatable">True to make field updatable</param>
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