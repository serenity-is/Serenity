using System;

namespace Serenity.ComponentModel
{
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category, bool collapsible = false, bool expanded = false)
        {
            Category = category;
            Collapsible = collapsible;
            Expanded = expanded;
        }

        public string Category { get; private set; }
        public bool Collapsible { get; private set; }
        public bool Expanded { get; private set; }
    }
}
