namespace Serenity.Data;

/// <summary>
///   Extension methods for classes implementing <see cref="IQueryWithParams"/>.
/// </summary>
public static class QueryWithParamsExtensions
{
    /// <summary>
    /// Freezes the query and its parameters, and prepares its SQL text for reuse.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <param name="self">The query.</param>
    /// <returns>The query itself.</returns>
    public static TQuery Freeze<TQuery>(this TQuery self) where TQuery : IQueryWithParams
    {
        ArgumentNullException.ThrowIfNull(self);
        self.Freeze();
        return self;
    }

    /// <summary>
    /// Freezes the query's parameters so they can no longer be added or changed.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <param name="self">The query.</param>
    /// <returns>The query itself.</returns>
    public static TQuery FreezeParams<TQuery>(this TQuery self) where TQuery : IQueryWithParams
    {
        ArgumentNullException.ThrowIfNull(self);
        self.FreezeParams();
        return self;
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <param name="self">The query.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="value">The value.</param>
    /// <returns>The query itself.</returns>
    public static TQuery SetParam<TQuery>(this TQuery self, Parameter param, object? value) where TQuery : IQueryWithParams
    {
        self.SetParam(param.Name, value);
        return self;
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <param name="self">The query.</param>
    /// <param name="value">The value.</param>
    /// <returns>The automatically named parameter that was added.</returns>
    public static Parameter AddParam<TQuery>(this TQuery self, object? value) where TQuery : IQueryWithParams
    {
        var param = self.AutoParam();
        self.AddParam(param.Name, value);
        return param;
    }

    /// <summary>
    /// Throws if a subquery cannot be safely embedded into another query, that is,
    /// when it is an independent query with auto parameters living in its own
    /// storage. Call sites must render the subquery first, as auto parameters
    /// only materialize at render time.
    /// </summary>
    /// <param name="subQuery">The subquery to embed.</param>
    /// <exception cref="InvalidOperationException">Subquery is independent and
    /// has auto parameters.</exception>
    internal const string UnsharedSubQueryMessage = "Cannot embed a query with auto parameters " +
        "into another query. Create the subquery with the outer query's SubQuery() method " +
        "so that parameters are shared.";

    /// <summary>
    /// Gets the root query sharing this instance's parameter storage, that is,
    /// itself when it has no parent, otherwise the topmost ancestor.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The root query.</returns>
    internal static IQueryWithParams GetRootQuery(this IQueryWithParams query)
    {
        var root = query;
        while (root.Parent is not null)
            root = root.Parent;
        return root;
    }

    internal static void ThrowIfUnsharedSubQueryWithAutoParams(this ISqlQuery subQuery)
    {
        if (subQuery.Parent is null && subQuery.HasAutoParams)
            throw new InvalidOperationException(UnsharedSubQueryMessage);
    }

    /// <summary>
    /// Throws if a subquery cannot be safely embedded into an outer query, that is,
    /// when it has auto parameters living in a different parameter storage.
    /// Unlike the criteria composition sites, the outer query is known here, so
    /// subqueries borrowed from another tree are rejected too, not just
    /// independent ones. Callers must render the subquery first, as auto
    /// parameters only materialize at render time.
    /// </summary>
    /// <param name="outer">The outer query.</param>
    /// <param name="subQuery">The subquery to embed.</param>
    /// <exception cref="InvalidOperationException">Subquery has auto parameters
    /// outside the outer query's parameter storage.</exception>
    internal static void ThrowIfUnsharedSubQueryWithAutoParams(this IQueryWithParams outer, IQueryWithParams subQuery)
    {
        if (!ReferenceEquals(outer.GetRootQuery(), subQuery.GetRootQuery()) &&
            subQuery.HasAutoParams)
            throw new InvalidOperationException(UnsharedSubQueryMessage);
    }
}