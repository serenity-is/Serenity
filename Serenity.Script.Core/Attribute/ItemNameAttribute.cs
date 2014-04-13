using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class ItemNameAttribute : Attribute
    {
        public ItemNameAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}