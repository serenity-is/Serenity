namespace Serenity.Data;

/// <summary>
/// RIGHT JOIN type
/// </summary>
/// <seealso cref="Join" />
public class RightJoin : Join
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RightJoin"/> class.
    /// </summary>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    public RightJoin(string toTable, string alias, ICriteria onCriteria)
        : base(null, toTable, alias, onCriteria)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RightJoin"/> class.
    /// </summary>
    /// <param name="joins">The joins dictionary.</param>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    public RightJoin(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
        : base(joins, toTable, alias, onCriteria)
    {
    }

    /// <summary>
    /// Gets the SQL keyword.
    /// </summary>
    /// <returns></returns>
    public override string GetKeyword()
    {
        return "RIGHT JOIN";
    }
}