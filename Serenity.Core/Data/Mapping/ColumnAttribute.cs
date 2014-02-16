using System;

namespace Serenity.Data.Mapping
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            if (name.IsTrimmedEmpty())
                throw new ArgumentNullException("name");

            this.Name = name;
        }

        public string Name { get; private set; }
    }
}