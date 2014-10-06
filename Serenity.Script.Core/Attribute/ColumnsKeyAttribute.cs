using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class ColumnsKeyAttribute : Attribute
    {
        public ColumnsKeyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}