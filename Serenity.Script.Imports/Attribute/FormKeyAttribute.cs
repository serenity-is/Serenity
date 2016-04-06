using jQueryApi.UI.Widgets;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class FormKeyAttribute : Attribute
    {
        public FormKeyAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}