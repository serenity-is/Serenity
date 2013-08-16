using System;

namespace Serenity.ComponentModel
{
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category)
        {
            Category = category;
        }

        public string Category { get; private set; }
    }
}
