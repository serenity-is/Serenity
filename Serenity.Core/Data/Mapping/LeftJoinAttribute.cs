using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class LeftJoinAttribute : Attribute
    {
        /// <summary>
        /// Adds a left join on foreign key. Use this version only on properties with ForeignKey attribute.
        /// </summary>
        /// <param name="alias">Foreign join alias</param>
        public LeftJoinAttribute(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Adds a left join
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="toTable"></param>
        /// <param name="onCriteria">On property, this parameter is a field name, while on class this is an ON criteria</param>
        public LeftJoinAttribute(string alias, string toTable, string onCriteria)
        {
            this.Alias = alias;
            this.ToTable = toTable;
            this.OnCriteria = onCriteria;
        }

        public String Alias { get; private set; }
        public String ToTable { get; private set; }
        public String OnCriteria { get; private set; }
    }

    [Obsolete("Use LeftJoinAttribute instead")]
    public class AddJoinAttribute : LeftJoinAttribute
    {
        public AddJoinAttribute(string alias)
            : base(alias)
        {
        }
    }

    [Obsolete("Use LeftJoinAttribute instead")]
    public class AddJoinToAttribute : LeftJoinAttribute
    {
        public AddJoinToAttribute(string alias, string toTable, string field)
            : base(alias, toTable, field)
        {
        }
    }

    [Obsolete("Use LeftJoinAttribute instead")]
    public class AddLeftJoinAttribute : LeftJoinAttribute
    {
        public AddLeftJoinAttribute(string alias, string toTable, string criteria)
            : base(alias, toTable, criteria)
        {
        }
    }
}