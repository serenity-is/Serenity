using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class CssClassAttribute : Attribute
    {
        public CssClassAttribute(string cssClass)
        {
            CssClass = cssClass;
        }

        [IntrinsicProperty]
        public string CssClass { get; private set; }
    }
}
