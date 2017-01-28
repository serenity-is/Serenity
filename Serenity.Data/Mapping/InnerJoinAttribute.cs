using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class InnerJoinAttribute : Attribute, ISqlJoin
    {
        /// <summary>
        /// Adds a inner join on foreign key. Use this version only on properties with ForeignKey attribute.
        /// </summary>
        /// <param name="alias">Foreign join alias</param>
        public InnerJoinAttribute(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Adds a inner join
        /// </summary>
        /// <param name="alias">Join alias</param>
        /// <param name="toTable">Join table</param>
        /// <param name="onCriteria">If the attribute is used on a property, this parameter is a field name, if used on a class,
        /// this parameter is the ON criteria of the inner join statement.</param>
        public InnerJoinAttribute(string alias, string toTable, string onCriteria)
        {
            this.Alias = alias;
            this.ToTable = toTable;
            this.OnCriteria = onCriteria;
        }

        public String Alias { get; private set; }
        public String ToTable { get; private set; }
        public String OnCriteria { get; private set; }
        public String Prefix { get; }
        public Type RowType { get; set; }
    }
}
