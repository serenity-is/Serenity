namespace Serenity.Data;

/// <summary>
///   Extension methods for classes implementing <see cref="IQueryWithParams"/>.
/// </summary>
public static class QueryWithParamsExtensions
{
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
}