using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class LocalTextPrefixAttribute : Attribute
    {
        public LocalTextPrefixAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}