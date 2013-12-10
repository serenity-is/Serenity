using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class NamePropertyAttribute : Attribute
    {
        public NamePropertyAttribute(string nameProperty)
        {
            this.NameProperty = nameProperty;
        }

        public string NameProperty { get; private set; }
    }
}