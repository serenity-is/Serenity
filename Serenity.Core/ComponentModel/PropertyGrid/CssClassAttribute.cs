using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets the CSS class for columns and form fields.
    /// In forms, class is added to container div with .field class that contains both label and editor.
    /// For columns, it sets cssClass property of SlickColumn, which adds this class to slick cells for all rows.
    /// Slick column headers are not affected by this attribute, use HeaderCssClass for that.
    /// </summary>
    public class CssClassAttribute : Attribute
    {
        public CssClassAttribute(string cssClass)
        {
            CssClass = cssClass;
        }

        public string CssClass { get; private set; }
    }
}