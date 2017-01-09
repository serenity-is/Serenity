using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets a placeholder for a form field. Placeholder text is shown inside the editor
    /// when its value is empty. Only editors using basic inputs and Select2 editor
    /// supports this.
    /// </summary>
    public class PlaceholderAttribute : Attribute
    {
        public PlaceholderAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
