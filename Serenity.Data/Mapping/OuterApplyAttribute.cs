using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OuterApplyAttribute : Attribute, ISqlJoin
    {
        public OuterApplyAttribute(string alias, string innerQuery)
        {
            this.Alias = alias;
            this.InnerQuery = innerQuery;
        }

        public String Alias { get; private set; }
        public String InnerQuery { get; private set; }
        public String Prefix { get; set; }
        public Type RowType { get; set; }

        string ISqlJoin.OnCriteria => InnerQuery;
        string ISqlJoin.ToTable => null;
    }
}