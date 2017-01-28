using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Explicitly specifies the database column name for property.
    /// Use this attribute if matching column name in database is different than the property name.
    /// </summary>
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