namespace Serenity.Data;

/// <summary>
/// CROSS APPLY type of join
/// </summary>
/// <seealso cref="Join" />
public class CrossApply : Join
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CrossApply"/> class.
    /// </summary>
    /// <param name="subQuery">Subquery.</param>
    /// <param name="alias">The alias.</param>
    public CrossApply(string subQuery, string alias)
        : base(null, string.IsNullOrEmpty(subQuery) ? subQuery : "(" + subQuery + ")", alias, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossApply"/> class.
    /// </summary>
    /// <param name="joins">Dictionary of joins.</param>
    /// <param name="subQuery">Subquery.</param>
    /// <param name="alias">The alias.</param>
    public CrossApply(IDictionary<string, Join> joins, string subQuery, string alias)
        : base(joins, string.IsNullOrEmpty(subQuery) ? subQuery : "(" + subQuery + ")", alias, null)
    {
    }

    /// <summary>
    /// Gets the SQL keyword.
    /// </summary>
    /// <returns></returns>
    public override string GetKeyword()
    {
        return "CROSS APPLY";
    }
}