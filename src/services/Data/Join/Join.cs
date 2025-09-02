namespace Serenity.Data;

/// <summary>
///   Corresponds to an SQL JOIN (INNER, OUTER, CROSS etc.)</summary>
public abstract class Join : Alias
{
    private readonly IDictionary<string, Join> joins;
    private readonly ICriteria onCriteria;
    private readonly HashSet<string> referencedAliases;

    /// <summary>
    /// Gets the keyword.
    /// </summary>
    /// <returns></returns>
    public abstract string GetKeyword();

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class.
    /// </summary>
    /// <param name="joins">The joins dictionary.</param>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    /// <exception cref="ArgumentException"></exception>
    protected Join(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
        : base(toTable, alias)
    {
        this.joins = joins;
        this.onCriteria = onCriteria;

        if (this.onCriteria is object)
        {
            var aliases = JoinAliasLocator.Locate(this.onCriteria.ToStringIgnoreParams());
            if (aliases != null && aliases.Count > 0)
                referencedAliases = aliases;
        }

        var toTableAliases = JoinAliasLocator.Locate(Table);
        if (toTableAliases != null && toTableAliases.Count > 0)
        {
            if (referencedAliases == null)
                referencedAliases = toTableAliases;
            else
            {
                foreach (var x in toTableAliases)
                    referencedAliases.Add(x);
            }
        }

        if (joins != null)
        {
            if (joins.ContainsKey(Name))
                throw new ArgumentException(string.Format(
                    "There is already a join with alias '{0}'", Name));

            joins.Add(Name, this);
        }
    }

    /// <summary>
    /// Gets the ON criteria.
    /// </summary>
    /// <value>
    /// The ON criteria.
    /// </value>
    public ICriteria OnCriteria => onCriteria;

    /// <summary>
    /// Gets the referenced aliases.
    /// </summary>
    /// <value>
    /// The referenced aliases.
    /// </value>
    public HashSet<string> ReferencedAliases => referencedAliases;

    /// <summary>
    /// Gets the joins.
    /// </summary>
    /// <value>
    /// The joins.
    /// </value>
    public IDictionary<string, Join> Joins => joins;

    /// <summary>
    /// Gets or sets the type of the row.
    /// </summary>
    /// <value>
    /// The type of the row.
    /// </value>
    public Type RowType { get; set; }
}