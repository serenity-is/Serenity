using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class FormKeyAttribute : Attribute
    {
        public FormKeyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}