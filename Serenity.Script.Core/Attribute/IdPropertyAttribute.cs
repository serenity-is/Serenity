using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class IdPropertyAttribute : Attribute
    {
        public IdPropertyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}