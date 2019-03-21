using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets a tab for a form field.
    /// </summary>
    public class TabAttribute : Attribute
    {
        public TabAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
