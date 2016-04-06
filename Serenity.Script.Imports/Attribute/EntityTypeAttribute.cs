using jQueryApi.UI.Widgets;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class EntityTypeAttribute : Attribute
    {
        public EntityTypeAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value  { get; private set; }
    }
}