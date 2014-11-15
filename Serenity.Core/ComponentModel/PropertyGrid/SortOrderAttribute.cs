using System;

namespace Serenity.ComponentModel
{
    public class SortOrderAttribute : Attribute
    {
        public SortOrderAttribute(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        public SortOrderAttribute(int sortOrder, bool descending)
            : this(descending ? -sortOrder : sortOrder)
        {
        }

        public int SortOrder { get; private set; }
        public bool Descending { get { return SortOrder < 0; } }
    }
}
