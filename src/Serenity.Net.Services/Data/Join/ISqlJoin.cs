namespace Serenity.Data.Mapping;

/// <summary>
/// Interface for SQL join types
/// </summary>
public interface ISqlJoin
{
    /// <summary>
    /// Gets the alias.
    /// </summary>
    /// <value>
    /// The alias.
    /// </value>
    string Alias { get; }

    /// <summary>
    /// Gets the table joined to
    /// </summary>
    /// <value>
    /// The table joined to.
    /// </value>
    string ToTable { get; }

    /// <summary>
    /// Gets the ON criteria.
    /// </summary>
    /// <value>
    /// The ON criteria.
    /// </value>
    string OnCriteria { get; }

    /// <summary>
    /// Gets the property prefix.
    /// </summary>
    /// <value>
    /// The property prefix.
    /// </value>
    string PropertyPrefix { get; }

    /// <summary>
    /// Gets or sets the title prefix.
    /// </summary>
    /// <value>
    /// The title prefix.
    /// </value>
    string TitlePrefix { get; set; }

    /// <summary>
    /// Gets or sets the type of the row.
    /// </summary>
    /// <value>
    /// The type of the row.
    /// </value>
    Type RowType { get; set; }

    /// <summary>
    /// Gets the dialect.
    /// </summary>
    public string Dialect { get; }
}