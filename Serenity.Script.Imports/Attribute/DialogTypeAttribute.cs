using jQueryApi.UI.Widgets;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class DialogTypeAttribute : Attribute
    {
        public DialogTypeAttribute(Type value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public Type Value { get; private set; }
    }
}