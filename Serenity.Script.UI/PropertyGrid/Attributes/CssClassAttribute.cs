using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
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
