using System;

namespace Serenity.Data.Mapping
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}