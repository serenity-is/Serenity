namespace Serenity.Data;

/// <summary>
/// LEFT JOIN type
/// </summary>
/// <seealso cref="Join" />
public class LeftJoin : Join
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeftJoin"/> class.
    /// </summary>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    public LeftJoin(string toTable, string alias, ICriteria onCriteria)
        : base(null, toTable, alias, onCriteria)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftJoin"/> class.
    /// </summary>
    /// <param name="joins">The joins dictionary.</param>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    public LeftJoin(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
        : base(joins, toTable, alias, onCriteria)
    {
    }

    /// <summary>
    /// Gets the SQL keyword.
    /// </summary>
    /// <returns></returns>
    public override string GetKeyword()
    {
        return "LEFT JOIN";
    }
}