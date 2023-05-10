namespace Serenity.Data.Mapping;

/// <summary>
/// Adds a OUTER APPLY to the row
/// </summary>
/// <seealso cref="Attribute" />
/// <seealso cref="ISqlJoin" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class OuterApplyAttribute : Attribute, ISqlJoin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OuterApplyAttribute"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="innerQuery">The inner query.</param>
    public OuterApplyAttribute(string alias, string innerQuery)
    {
        Alias = alias;
        InnerQuery = innerQuery;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OuterApplyAttribute"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="innerQuery">The inner query.</param>
    /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
    public OuterApplyAttribute(string alias, string innerQuery, params ServerType[] serverTypes)
        : this(alias, innerQuery)
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
    /// Gets the inner query.
    /// </summary>
    /// <value>
    /// The inner query.
    /// </value>
    public string InnerQuery { get; private set; }

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

    string ISqlJoin.OnCriteria => InnerQuery;
    string ISqlJoin.ToTable => null;

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