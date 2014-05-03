using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class CssClassAttribute : Attribute
    {
        public CssClassAttribute(string cssClass)
        {
            CssClass = cssClass;
        }

        public string CssClass { get; private set; }
    }
}