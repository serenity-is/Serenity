using System.Diagnostics;
using Dictionary = System.Collections.Generic.Dictionary<string, object?>;

namespace Serenity.Data;

/// <summary>
/// Base class for queries with params like SqlQuery, SqlUpdate, SqlInsert.
/// </summary>
/// <seealso cref="IQueryWithParams" />
[DebuggerDisplay("{DebugText}")]
public class QueryWithParams : IQueryWithParams
{
    private System.Collections.Generic.Dictionary<string, string>? aliasExpressions;

    /// <summary>
    /// The dialect.
    /// </summary>
    protected ISqlDialect dialect;

    /// <summary>
    /// Is the dialect overridden.
    /// </summary>
    protected bool dialectOverridden;

    /// <summary>
    /// The parent query with param storage.
    /// </summary>
    protected QueryWithParams? parent;

    /// <summary>
    /// The parameters.
    /// </summary>
    protected Dictionary? parameters;

    /// <summary>
    /// The next auto param counter.
    /// </summary>
    protected int nextAutoParam;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryWithParams"/> class.
    /// </summary>
    public QueryWithParams()
    {
        dialect = SqlSettings.DefaultDialect;
    }

    /// <summary>
    /// Clones the parameters into a target query.
    /// </summary>
    /// <param name="target">The target.</param>
    protected void CloneParams(QueryWithParams target)
    {
        if (parameters != null)
        {
            var p = new Dictionary();
            foreach (var pair in parameters)
                p.Add(pair.Key, pair.Value);

            target.parameters = p;
        }
        else {
            target.parameters = null;
        }
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void AddParam(string name, object? value)
    {
        if (parent != null)
        {
            parent.AddParam(name, value);
            return;
        }

        parameters ??= [];

        parameters.Add(name, value);
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void SetParam(string name, object? value)
    {
        if (parent != null)
        {
            parent.SetParam(name, value);
            return;
        }

        parameters ??= [];

        parameters[name] = value;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    public IDictionary<string, object?>? Params
    {
        get
        {
            if (parent != null)
                return parent.Params;

            return parameters;
        }
    }

    /// <summary>
    /// Gets the parameter count.
    /// </summary>
    /// <value>
    /// The parameter count.
    /// </value>
    public int ParamCount
    {
        get
        {
            if (parent != null)
                return parent.ParamCount;

            return parameters == null ? 0 : parameters.Count;
        }
    }

    /// <summary>
    /// Creates an automatically named parameter.
    /// </summary>
    /// <returns>The automatically named parameter.</returns>
    public Parameter AutoParam()
    {
        if (parent != null)
            return parent.AutoParam();

        return new Parameter((++nextAutoParam).IndexParam());
    }

    /// <summary>
    /// Creates a new query that shares parameter dictionary with this query.
    /// </summary>
    /// <returns>
    /// A new query that shares parameters.
    /// </returns>
    public TQuery CreateSubQuery<TQuery>()
        where TQuery : QueryWithParams, new()
    {
        return new TQuery
        {
            parent = this
        };
    }

    /// <summary>
    /// Gets the source expression registered for an alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>The source expression, or null when the alias is not registered.</returns>
    protected string? GetAliasExpression(string alias)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (parent is not null)
            return parent.GetAliasExpression(alias);

        return aliasExpressions is not null && aliasExpressions.TryGetValue(alias, out var expression) ? expression : null;
    }

    /// <summary>
    /// Gets a value indicating whether this query tree has an alias registered.
    /// </summary>
    /// <param name="alias">The alias to check.</param>
    public bool HasAlias(string alias)
    {
        if (string.IsNullOrEmpty(alias))
            return false;

        if (parent is not null)
            return parent.HasAlias(alias);

        return aliasExpressions is not null && aliasExpressions.ContainsKey(alias);
    }

    /// <summary>
    /// Sets the source expression for an alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="expression">The source expression.</param>
    protected void SetAliasExpression(string alias, string expression)
    {
        ArgumentNullException.ThrowIfNull(alias);
        ArgumentNullException.ThrowIfNull(expression);

        if (parent is not null)
        {
            parent.SetAliasExpression(alias, expression);
            return;
        }

        aliasExpressions ??= new(StringComparer.OrdinalIgnoreCase);
        aliasExpressions[alias] = expression;
    }

    /// <summary>
    /// Clears the alias expressions registered in this query. If <paramref name="localOnly"/> is false, 
    /// clears the alias expressions in the root query.
    /// </summary>
    protected void ResetAliasExpressions(bool localOnly)
    {
        if (parent is not null && !localOnly)
        {
            parent.ResetAliasExpressions(localOnly: false);
            return;
        }

        aliasExpressions = null;
    }

    /// <summary>
    /// Copies this query's local alias expressions to another query.
    /// </summary>
    /// <param name="target">The target query.</param>
    protected void CloneAliasExpressionsTo(QueryWithParams target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (parent is null && target.parent is null && aliasExpressions is not null)
            target.aliasExpressions = new(aliasExpressions, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets an automatically generated alias that is not already used in this query tree.
    /// </summary>
    /// <returns>The alias.</returns>
    internal string AutoAlias()
    {
        if (parent is not null)
            return parent.AutoAlias();

        var index = 1;
        string candidate;
        do
        {
            candidate = "T" + index++;
        }
        while (aliasExpressions is not null && aliasExpressions.ContainsKey(candidate));

        return candidate;
    }

    ISqlDialect IQueryWithParams.Dialect => dialect;

    /// <summary>
    /// Gets the dialect (SQL server type / version) for query.
    /// </summary>
    public ISqlDialect Dialect()
    {
        return dialect;
    }

    /// <summary>
    /// Gets a value indicating whether the dialect is overridden.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the dialect is overridden; otherwise, <c>false</c>.
    /// </value>
    public bool IsDialectOverridden => dialectOverridden;

    /// <summary>
    /// Gets the debug text.
    /// </summary>
    /// <value>
    /// The debug text.
    /// </value>
    public string? DebugText => SqlDebugDumper.Dump(ToString(), Params, dialect);
}