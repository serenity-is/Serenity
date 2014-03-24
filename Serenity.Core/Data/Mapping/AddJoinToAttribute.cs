using System;

namespace Serenity.Data.Mapping
{
    public class AddJoinToAttribute : Attribute
    {
        public AddJoinToAttribute(string alias, string toTable, string toField)
        {
            this.Alias = alias;
            this.ToTable = ToTable;
            this.ToField = ToField;
        }

        public String Alias { get; private set; }
        public String ToTable { get; private set; }
        public String ToField { get; private set; }
    }
}