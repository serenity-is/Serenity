using System;

namespace Serenity.Data.Mapping
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.Name = name;
        }

        public string Name { get; private set; }
    }
}