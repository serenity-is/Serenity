using System;

namespace Serenity.Data.Mapping
{
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string table, string field)
        {
            this.Field = field;
            this.Table = table;
        }

        public string Field { get; private set; }
        public string Table { get; private set; }
    }
}