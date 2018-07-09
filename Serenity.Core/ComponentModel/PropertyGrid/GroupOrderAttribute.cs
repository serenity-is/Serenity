using System;

namespace Serenity.ComponentModel
{
    public class GroupOrderAttribute : Attribute
    {
        public GroupOrderAttribute(int groupOrder)
        {
            GroupOrder = groupOrder;
        }

        public int GroupOrder { get; private set; }
    }
}
