using System;

namespace Serenity.ComponentModel
{
    public sealed class EditLinkAttribute : Attribute
    {
        public EditLinkAttribute()
        {
            this.Value = true;
        }

        public EditLinkAttribute(bool value)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }

        public string ItemType { get; set; }
        public string IdField { get; set; }
        public string CssClass { get; set; }
    }
}