using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BasedOnRowAttribute : Attribute
    {
        public BasedOnRowAttribute(Type rowType)
        {
            RowType = rowType;
        }

        public Type RowType { get; private set; }
        public bool CheckNames { get; set; }
    }
}
