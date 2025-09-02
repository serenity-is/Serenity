namespace Serenity.Data.Mapping;

/// <summary>
/// INNER JOIN type
/// </summary>
/// <seealso cref="Attribute" />
/// <seealso cref="ISqlJoin" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class InnerJoinAttribute : Attribute, ISqlJoin
{
    /// <summary>
    /// Adds a inner join on foreign key. Use this version only on properties with ForeignKey attribute.
    /// </summary>
    /// <param name="alias">Foreign join alias</param>
    public InnerJoinAttribute(string alias)
    {
        Alias = alias;
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
        Alias = alias;
        ToTable = toTable;
        OnCriteria = onCriteria;
    }

    /// <summary>
    /// Adds a inner join on foreign key. Use this version only on properties with ForeignKey attribute.
    /// </summary>
    /// <param name="alias">Foreign join alias</param>
    /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
    public InnerJoinAttribute(string alias, params ServerType[] serverTypes)
        : this(alias)
    {
        Dialect = string.Join(",", serverTypes);
    }

    /// <summary>
    /// Adds a inner join
    /// </summary>
    /// <param name="alias">Join alias</param>
    /// <param name="toTable">Join table</param>
    /// <param name="onCriteria">If the attribute is used on a property, this parameter is a field name, if used on a class,
    /// this parameter is the ON criteria of the inner join statement.</param>
    /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
    public InnerJoinAttribute(string alias, string toTable, string onCriteria, params ServerType[] serverTypes)
        : this(alias, toTable, onCriteria)
    {
        Dialect = string.Join(",", serverTypes);
    }

    /// <summary>
    /// Gets the alias.
    /// </summary>
    /// <value>
    /// The alias.
    /// </value>
    public string Alias { get; private set; }

    /// <summary>
    /// Gets the table.
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

    /// <summary>
    /// Gets or sets the dialect.
    /// </summary>
    /// <value>
    /// The dialect.
    /// </value>
    public string Dialect { get; set; }

    /// <summary>
    /// Gets or sets the negating of the dialect.
    /// </summary>
    /// <value>
    /// The negating of the dialect.
    /// </value>
    public bool NegateDialect
    {
        get => Dialect != null && Dialect.StartsWith('!');
        set => Dialect = value ? (!NegateDialect ? ("!" + Dialect) : Dialect) :
            (NegateDialect ? Dialect[1..] : Dialect);
    }
}
