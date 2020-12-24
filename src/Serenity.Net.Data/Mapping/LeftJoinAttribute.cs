using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// LEFT JOIN type
    /// </summary>
    /// <seealso cref="Attribute" />
    /// <seealso cref="ISqlJoin" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class LeftJoinAttribute : Attribute, ISqlJoin
    {
        /// <summary>
        /// Adds a left join on foreign key. Use this version only on properties with ForeignKey attribute.
        /// </summary>
        /// <param name="alias">Foreign join alias</param>
        public LeftJoinAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// Adds a left join
        /// </summary>
        /// <param name="alias">Join alias</param>
        /// <param name="toTable">Join table</param>
        /// <param name="onCriteria">If the attribute is used on a property, this parameter is a field name, if used on a class,
        /// this parameter is the ON criteria of the left join statement.</param>
        public LeftJoinAttribute(string alias, string toTable, string onCriteria)
        {
            Alias = alias;
            ToTable = toTable;
            OnCriteria = onCriteria;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the table joined to.
        /// </summary>
        /// <value>
        /// The table joined to.
        /// </value>
        public string ToTable { get; private set; }

        /// <summary>
        /// Gets the ON criteria.
        /// </summary>
        /// <value>
        /// The ON criteria.
        /// </value>
        public string OnCriteria { get; private set; }

        /// <summary>
        /// Gets the property prefix.
        /// </summary>
        /// <value>
        /// The property prefix.
        /// </value>
        public string PropertyPrefix { get; set; }

        /// <summary>
        /// Gets or sets the title prefix.
        /// </summary>
        /// <value>
        /// The title prefix.
        /// </value>
        public string TitlePrefix { get; set; }

        /// <summary>
        /// Gets or sets the type of the row.
        /// </summary>
        /// <value>
        /// The type of the row.
        /// </value>
        public Type RowType { get; set; }
    }
}