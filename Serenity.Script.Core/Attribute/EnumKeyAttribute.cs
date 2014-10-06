using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class EnumKeyAttribute : Attribute
    {
        public EnumKeyAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}