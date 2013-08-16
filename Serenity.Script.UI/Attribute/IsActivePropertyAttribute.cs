using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class IsActivePropertyAttribute : Attribute
    {
        public IsActivePropertyAttribute(string isActiveProperty)
        {
            this.IsActiveProperty = isActiveProperty;
        }

        public string IsActiveProperty { get; private set; }
    }
}