using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string category)
        {
            Category = category;
        }

        [IntrinsicProperty]
        public string Category { get; private set; }
    }
}

namespace Serenity.ComponentModel
{
}