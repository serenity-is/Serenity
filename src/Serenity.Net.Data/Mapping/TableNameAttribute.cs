namespace Serenity.Data.Mapping;

/// <summary>
/// Determines table name for the row.
/// </summary>
/// <seealso cref="Attribute" />
public class TableNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <exception cref="ArgumentNullException">name</exception>
    public TableNameAttribute(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");

        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
    /// <exception cref="ArgumentNullException">name</exception>
    public TableNameAttribute(string name, params ServerType[] serverTypes)
        : this(name)
    {
        Dialect = string.Join(",", serverTypes);
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; private set; }

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