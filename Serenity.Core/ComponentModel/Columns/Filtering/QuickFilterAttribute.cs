using System;

namespace Serenity.ComponentModel
{
    public class QuickFilterAttribute : Attribute
    {
        public QuickFilterAttribute(bool value = true)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }
        public bool Separator { get; set; }
        public string CssClass { get; set; }
    }
}
