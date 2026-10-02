namespace Serenity.Data;

/// <summary>
/// OUTER APPLY join type.
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
        : base(null, WrapSubQuery(innerQuery), alias, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OuterApply"/> class.
    /// </summary>
    /// <param name="joins">The joins.</param>
    /// <param name="innerQuery">The inner query.</param>
    /// <param name="alias">The alias.</param>
    public OuterApply(IDictionary<string, Join> joins, string innerQuery, string alias)
        : base(joins, WrapSubQuery(innerQuery), alias, null)
    {
    }

    /// <summary>
    /// Gets the SQL keyword.
    /// </summary>
    /// <returns>The SQL keyword for this join type.</returns>
    public override string GetKeyword()
    {
        return "OUTER APPLY";
    }

    private static string WrapSubQuery(string innerQuery)
    {
        ArgumentException.ThrowIfNullOrEmpty(innerQuery);
        return "(" + innerQuery + ")";
    }
}
