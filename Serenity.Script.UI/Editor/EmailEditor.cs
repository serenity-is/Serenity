using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("E-posta")]
    [Element("<input type=\"text\"/>")]
    public class EmailEditor : Widget<object>, IStringValue
    {
        public EmailEditor(jQueryObject input)
            : base(input, new object())
        {
            input.AddClass("email");
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
        }
    }
}