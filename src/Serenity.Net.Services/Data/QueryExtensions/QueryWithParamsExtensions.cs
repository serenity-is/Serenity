namespace Serenity.Data;

/// <summary>
///   Extension methods for classes implementing IDbWhereParam interface.</summary>
public static class QueryWithParamsExtensions
{
    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">The query.</param>
    /// <param name="param">The parameter.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static T SetParam<T>(this T self, Parameter param, object value) where T : IQueryWithParams
    {
        self.SetParam(param.Name, value);
        return self;
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">The query.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static Parameter AddParam<T>(this T self, object value) where T : IQueryWithParams
    {
        var param = self.AutoParam();
        self.AddParam(param.Name, value);
        return param;
    }
}