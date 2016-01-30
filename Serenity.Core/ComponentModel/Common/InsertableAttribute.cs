using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls if this field is editable in new record mode.
    /// When used with fields, turns on or off the insertable flag.
    /// </summary>
    public class InsertableAttribute : Attribute
    {
        /// <summary>
        /// Controls if this field is editable in new record mode.
        /// When used with fields, turns on or off the insertable flag.
        /// </summary>
        /// <param name="insertable">True to make field insertable</param>
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