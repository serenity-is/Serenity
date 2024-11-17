namespace Serenity.Data;

/// <summary>
///   Interface for objects setting parameters by PARAM method (like SqlInsert, SqlUpdate, SqlDelete...)</summary>
public interface IQueryWithParams
{
    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    void AddParam(string name, object value);

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    void SetParam(string name, object value);

    /// <summary>
    /// Creates an automatically named parameter.
    /// </summary>
    /// <returns></returns>
    Parameter AutoParam();

    /// <summary>
    /// Gets the dialect.
    /// </summary>
    /// <value>
    /// The dialect.
    /// </value>
    ISqlDialect Dialect { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    IDictionary<string, object> Params { get; }
}