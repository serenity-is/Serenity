using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets a placeholder for a form field.
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
