using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class IdPropertyAttribute : Attribute
    {
        public IdPropertyAttribute(string idProperty)
        {
            this.IdProperty = idProperty;
        }

        public string IdProperty { get; private set; }
    }
}