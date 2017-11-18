using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets CSS class for field on forms only. 
    /// </summary>
    public class FormCssClassAttribute : Attribute
    {
        public FormCssClassAttribute(string cssClass)
        {
            Value = cssClass;
        }

        public string Value { get; private set; }

        /// <summary>
        /// Applies this form css class to all following fields 
        /// until next another FormCssClass attribute
        /// </summary>
        public bool UntilNext { get; set; }
    }
}