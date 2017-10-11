using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets CSS class for field on forms only. Useful for Bootstrap grid, e.g. col-md-4 etc.
    /// </summary>
    public class FormCssClassAttribute : Attribute
    {
        public FormCssClassAttribute(string cssClass)
        {
            Value = cssClass;
        }

        public string Value { get; private set; }
    }
}