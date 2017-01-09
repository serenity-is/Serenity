using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets the CSS class for grid column headers. It sets headerCssClass property of SlickColumn.
    /// This has no effect for forms.
    /// </summary>
    public class HeaderCssClassAttribute : Attribute
    {
        public HeaderCssClassAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}