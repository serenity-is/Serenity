using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AddJoinToAttribute : Attribute
    {
        public AddJoinToAttribute(string alias, string toTable, string toField)
        {
            this.Alias = alias;
            this.ToTable = toTable;
            this.ToField = toField;
        }

        public String Alias { get; private set; }
        public String ToTable { get; private set; }
        public String ToField { get; private set; }
    }
}