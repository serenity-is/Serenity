using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class DialogTypeAttribute : Attribute
    {
        public DialogTypeAttribute(Type value)
        {
            this.Value = value;
        }

        public Type Value { get; private set; }
    }
}