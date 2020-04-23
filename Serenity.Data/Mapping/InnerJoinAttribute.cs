using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// INNER JOIN type
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="Serenity.Data.Mapping.ISqlJoin" />
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

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public String Alias { get; private set; }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>
        /// The table joined to.
        /// </value>
        public String ToTable { get; private set; }

        /// <summary>
        /// Gets the ON criteria.
        /// </summary>
        /// <value>
        /// The ON criteria.
        /// </value>
        public String OnCriteria { get; private set; }

        /// <summary>
        /// Gets the property prefix.
        /// </summary>
        /// <value>
        /// The property prefix.
        /// </value>
        public String PropertyPrefix { get; set; }

        /// <summary>
        /// Gets or sets the title prefix.
        /// </summary>
        /// <value>
        /// The title prefix.
        /// </value>
        public String TitlePrefix { get; set; }

        /// <summary>
        /// Gets or sets the type of the row.
        /// </summary>
        /// <value>
        /// The type of the row.
        /// </value>
        public Type RowType { get; set; }
    }
}
