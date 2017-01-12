using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this property is a foreign key to another field in a primary key table.
    /// There is no need for foreign key to exist in database actually. It is not checked.
    /// This is mostly used for joins.
    /// </summary>
    public class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// Specifies that this property is a foreign key to another field in a primary key table.
        /// </summary>
        /// <param name="table">Primary key table</param>
        /// <param name="field">Matching column in primary key table</param>
        public ForeignKeyAttribute(string table, string field = "Id")
        {
            this.Field = field;
            this.Table = table;
        }

        public string Field { get; private set; }
        public string Table { get; private set; }
    }
}