using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class NamePropertyAttribute : Attribute
    {
        public NamePropertyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}