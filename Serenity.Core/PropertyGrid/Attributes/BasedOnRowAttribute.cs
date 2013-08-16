using System;

namespace Serenity.ComponentModel
{
    public sealed class BasedOnRowAttribute : Attribute
    {
        public BasedOnRowAttribute(Type rowType)
        {
            RowType = rowType;
        }

        public Type RowType { get; private set; }
    }
}
