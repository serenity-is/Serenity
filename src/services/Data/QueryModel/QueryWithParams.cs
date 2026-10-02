using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Serenity.Data;

/// <summary>
/// Base class for queries with params like SqlQuery, SqlUpdate, SqlInsert.
/// </summary>
/// <seealso cref="IQueryWithParams" />
[DebuggerDisplay("{DebugText}")]
public class QueryWithParams : IQueryWithParams
{
    private Dictionary<string, string>? aliasExpressions;
    private bool isFrozen;

    /// <summary>
    /// The cached SQL string and the dialect it was formatted with.
    /// </summary>
    protected (string Sql, ISqlDialect Dialect)? cachedToString;

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
    protected IDictionary<string, object?>? parameters;

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
        ArgumentNullException.ThrowIfNull(target);
        target.BeforeModify();

        // A subquery clone stays attached to the same parameter root. Root
        // clones instead receive an independent, mutable copy of the values.
        target.parent = parent;

        if (parent is null)
        {
            target.dialect = dialect;
            target.dialectOverridden = dialectOverridden;
            target.nextAutoParam = nextAutoParam;
        }

        if (parent is not null || parameters is null)
            target.parameters = null;
        else
        {
            var cloned = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in parameters)
                cloned.Add(pair.Key, pair.Value);
            target.parameters = cloned;
        }
    }

    /// <summary>
    /// Invalidates this query's cached SQL and that of its parent query.
    /// </summary>
    protected void BeforeModify()
    {
        parent?.BeforeModify();

        if (isFrozen)
            throw new InvalidOperationException("Query has been frozen.");

        cachedToString = null;
    }

    /// <summary>
    /// Gets the cached SQL string, regenerating it when this query tree's dialect changes.
    /// </summary>
    /// <param name="format">The formatter.</param>
    /// <returns>The SQL string.</returns>
    protected string GetCachedToString(Func<ISqlDialect, string> format)
    {
        ArgumentNullException.ThrowIfNull(format);

        var currentDialect = Dialect();
        if (cachedToString is not { } cached || !ReferenceEquals(cached.Dialect, currentDialect))
            cachedToString = (format(currentDialect), currentDialect);

        return cachedToString.Value.Sql;
    }

    private static void CheckParamName(string name)
    {
        Parameter.CheckName(name);
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="name">The name. Must start with "@".</param>
    /// <param name="value">The value.</param>
    public void AddParam(string name, object? value)
    {
        CheckParamName(name);

        if (parent != null)
        {
            parent.AddParam(name, value);
            return;
        }

        if (IsParamsFrozen)
            throw new InvalidOperationException("Query parameters have been frozen.");

        parameters ??= new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (parameters.ContainsKey(name))
            throw new ArgumentException($"A parameter named '{name}' has already been added.", nameof(name));

        parameters.Add(name, value);
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="name">The name. Must start with "@".</param>
    /// <param name="value">The value.</param>
    public void SetParam(string name, object? value)
    {
        CheckParamName(name);

        if (parent != null)
        {
            parent.SetParam(name, value);
            return;
        }

        if (IsParamsFrozen)
            throw new InvalidOperationException("Query parameters have been frozen.");

        (parameters ??= new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase))[name] = value;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    public IReadOnlyDictionary<string, object?>? Params
    {
        get
        {
            if (parent != null)
                return parent.Params;

            return parameters is null ? null : new ReadOnlyDictionary<string, object?>(parameters);
        }
    }

    /// <summary>
    /// Gets a value indicating whether parameters have been frozen.
    /// </summary>
    public bool IsParamsFrozen => parent?.IsParamsFrozen ?? parameters?.IsReadOnly == true;

    /// <summary>
    /// Gets a value indicating whether this query or one of its parents has been frozen.
    /// </summary>
    public bool IsFrozen => isFrozen || parent?.IsFrozen == true;

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
    /// Gets a value indicating whether any automatically named parameters
    /// were generated for this query or one of the queries sharing its
    /// parameter storage. Derived from the auto param counter, so no
    /// separate tracking state is needed. Note that this may still return
    /// false before the query is rendered (e.g. via ToString()), as auto
    /// parameters only materialize at render time.
    /// </summary>
    public bool HasAutoParams => parent?.HasAutoParams ?? nextAutoParam > 0;

    /// <summary>
    /// Gets access to parent query if any.
    /// </summary>
    public IQueryWithParams? Parent => parent;

    /// <summary>
    /// Creates an automatically named parameter.
    /// </summary>
    /// <returns>The automatically named parameter.</returns>
    public Parameter AutoParam()
    {
        if (parent != null)
            return parent.AutoParam();

        if (IsFrozen)
            throw new InvalidOperationException("Query has been frozen.");

        if (IsParamsFrozen)
            throw new InvalidOperationException("Query parameters have been frozen.");

        return new Parameter((++nextAutoParam).IndexParam());
    }

    /// <summary>
    /// Creates a new query that shares parameter storage and dialect with this query tree.
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
    /// Sets the dialect for this query tree, propagating the change to its root.
    /// </summary>
    /// <param name="value">The dialect.</param>
    protected void SetDialect(ISqlDialect value)
    {
        ArgumentNullException.ThrowIfNull(value);
        BeforeModify();

        if (parent is not null)
        {
            parent.SetDialect(value);
            return;
        }

        dialect = value;
        dialectOverridden = true;
    }

    void IQueryWithParams.Freeze()
    {
        if (parent is not null)
            throw new InvalidOperationException("Freeze cannot be called on a subquery.");

        _ = ToString();

        if (isFrozen)
            return;

        ((IQueryWithParams)this).FreezeParams();
        isFrozen = true;
    }

    void IQueryWithParams.FreezeParams()
    {
        if (parent is not null)
            throw new InvalidOperationException("FreezeParams cannot be called on a subquery.");

        if (parameters?.IsReadOnly == true)
            return;

        parameters = (parameters ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)).AsReadOnly();
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

        BeforeModify();

        SetAliasExpressionCore(alias, expression);
    }

    private void SetAliasExpressionCore(string alias, string expression)
    {
        if (parent is not null)
        {
            parent.SetAliasExpressionCore(alias, expression);
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
        BeforeModify();

        ResetAliasExpressionsCore(localOnly);
    }

    private void ResetAliasExpressionsCore(bool localOnly)
    {
        if (parent is not null && !localOnly)
        {
            parent.ResetAliasExpressionsCore(localOnly: false);
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

        // Aliases on child queries resolve through their parent. In particular,
        // UNION legs share the root registry, so copying the root registry into
        // a child clone would give it a redundant and potentially stale snapshot.
        if (parent is null && target.parent is null && aliasExpressions is not null)
        {
            target.BeforeModify();
            target.aliasExpressions = new(aliasExpressions, StringComparer.OrdinalIgnoreCase);
        }
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

    ISqlDialect IQueryWithParams.Dialect => Dialect();

    /// <summary>
    /// Gets the dialect (SQL server type / version) for query.
    /// </summary>
    public ISqlDialect Dialect()
    {
        return parent?.Dialect() ?? dialect;
    }

    /// <summary>
    /// Gets a value indicating whether the dialect is overridden.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the dialect is overridden; otherwise, <c>false</c>.
    /// </value>
    public bool IsDialectOverridden => parent?.IsDialectOverridden ?? dialectOverridden;

    /// <summary>
    /// Gets the debug text.
    /// </summary>
    /// <value>
    /// The debug text.
    /// </value>
    public string? DebugText => SqlDebugDumper.Dump(ToString(), Params, Dialect());
}
