using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddLeftJoinAttribute : Attribute
    {
        public AddLeftJoinAttribute(string alias, string toTable, string onCriteria)
        {
            this.Alias = alias;
            this.ToTable = toTable;
            this.OnCriteria = onCriteria;
        }

        public String Alias { get; private set; }
        public String ToTable { get; private set; }
        public String OnCriteria { get; private set; }
    }
}