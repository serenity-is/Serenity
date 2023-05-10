using System.Diagnostics;
using Dictionary = System.Collections.Generic.Dictionary<string, object>;

namespace Serenity.Data;

/// <summary>
/// Base class for queries with params like SqlQuery, SqlUpdate, SqlInsert
/// </summary>
/// <seealso cref="IQueryWithParams" />
[DebuggerDisplay("{DebugText}")]
public class QueryWithParams : IQueryWithParams
{
    /// <summary>
    /// The dialect
    /// </summary>
    protected ISqlDialect dialect;

    /// <summary>
    /// Is the dialect overridden
    /// </summary>
    protected bool dialectOverridden;

    /// <summary>
    /// The parent query with param storage
    /// </summary>
    protected QueryWithParams parent;

    /// <summary>
    /// The parameters
    /// </summary>
    protected Dictionary parameters;

    /// <summary>
    /// The next auto param counter
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

        target.parameters = null;
    }

    /// <summary>
    /// Adds the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void AddParam(string name, object value)
    {
        if (parent != null)
        {
            parent.AddParam(name, value);
            return;
        }

        if (parameters == null)
            parameters = new Dictionary();

        parameters.Add(name, value);
    }

    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void SetParam(string name, object value)
    {
        if (parent != null)
        {
            parent.SetParam(name, value);
            return;
        }

        if (parameters == null)
            parameters = new Dictionary();

        parameters[name] = value;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <value>
    /// The parameters.
    /// </value>
    public IDictionary<string, object> Params
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
    /// <returns></returns>
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
    /// A new query that shares parameters.</returns>
    public TQuery CreateSubQuery<TQuery>()
        where TQuery : QueryWithParams, new()
    {
        var subQuery = new TQuery
        {
            parent = this
        };
        return subQuery;
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
    public string DebugText => SqlDebugDumper.Dump(ToString(), Params, dialect);
}