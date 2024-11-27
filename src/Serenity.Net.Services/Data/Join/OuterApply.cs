namespace Serenity.Data;

/// <summary>
/// OUTER APPLY join type
/// </summary>
/// <seealso cref="Join" />
public class OuterApply : Join
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OuterApply"/> class.
    /// </summary>
    /// <param name="innerQuery">The inner query.</param>
    /// <param name="alias">The alias.</param>
    public OuterApply(string innerQuery, string alias)
        : base(null, string.IsNullOrEmpty(innerQuery) ? innerQuery : "(" + innerQuery + ")", alias, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OuterApply"/> class.
    /// </summary>
    /// <param name="joins">The joins.</param>
    /// <param name="innerQuery">The inner query.</param>
    /// <param name="alias">The alias.</param>
    public OuterApply(IDictionary<string, Join> joins, string innerQuery, string alias)
        : base(joins, string.IsNullOrEmpty(innerQuery) ? innerQuery : "(" + innerQuery + ")", alias, null)
    {
    }

    /// <summary>
    /// Gets the SQL keyword.
    /// </summary>
    /// <returns></returns>
    public override string GetKeyword()
    {
        return "OUTER APPLY";
    }
}