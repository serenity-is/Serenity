using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category, bool collapsible = false, bool expanded = false)
        {
            Category = category;
            Collapsible = collapsible;
            Expanded = expanded;
        }

        [IntrinsicProperty]
        public string Category { get; private set; }
        public bool Collapsible { get; private set; }
        public bool Expanded { get; private set; }
    }
}

namespace Serenity.ComponentModel
{
}