using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Serenity.Data;

/// <summary>
/// The base criteria object type, from which all criteria types derive.
/// </summary>
/// <seealso cref="ICriteria" />
[DebuggerDisplay("{ToStringIgnoreParams()}")]
[JsonConverter(typeof(JsonConverters.CriteriaJsonConverter))]
[Newtonsoft.Json.JsonConverter(typeof(JsonCriteriaConverter))]
public abstract class BaseCriteria : ICriteria
{
    private static readonly NoParamsChecker noParamsChecker = new();

    /// <summary>
    /// Gets a value indicating whether this criteria instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsEmpty => false;

    /// <summary>
    /// Creates a new unary IsNull criteria containing this criteria as the operand.
    /// </summary>
    /// <returns>A new unary IsNull criteria.</returns>
    public BaseCriteria IsNull()
    {
        return new UnaryCriteria(CriteriaOperator.IsNull, this);
    }

    /// <summary> 
    /// Creates a new unary IsNotNull criteria containing this criteria as the operand.
    /// </summary>
    /// <returns>A new unary IsNotNull criteria.</returns>
    public BaseCriteria IsNotNull()
    {
        return new UnaryCriteria(CriteriaOperator.IsNotNull, this);
    }

    /// <summary>
    /// Creates a new binary Like criteria containing this criteria as the left operand.
    /// The mask is a raw LIKE pattern: %, _ and [...] act as wildcards and are not escaped.
    /// For literal values see <see cref="Contains"/>, <see cref="StartsWith"/> or
    /// <see cref="EndsWith"/>.
    /// </summary>
    /// <param name="mask">The raw LIKE pattern.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Like criteria.</returns>
    public BaseCriteria Like(string mask, bool upper = false)
    {
        var left = this;
        if (upper)
            left = new UpperFunctionCriteria(left);
        BaseCriteria right = new ValueCriteria(mask);
        if (upper)
            right = new UpperFunctionCriteria(right);
        return new BinaryCriteria(left, CriteriaOperator.Like, right);
    }

    /// <summary>
    /// Creates a new binary Like criteria containing this criteria as the left operand,
    /// with an ESCAPE clause. The mask must already be escaped for the given escape
    /// character (see <see cref="Criteria.EscapeLikeWildcards"/>); it is used as is.
    /// For literal values prefer <see cref="Contains"/>, <see cref="StartsWith"/> or
    /// <see cref="EndsWith"/>; for raw patterns without ESCAPE see
    /// <see cref="Like(string, bool)"/>.
    /// </summary>
    /// <param name="mask">The LIKE mask, already escaped for the given escape character.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <param name="escape">The LIKE escape character, appended as ESCAPE 'x'. Default is '!'.</param>
    /// <returns>A new binary Like criteria.</returns>
    public BaseCriteria LikeEscaped(string mask, bool upper = false, char escape = '!')
    {
        var left = this;
        if (upper)
            left = new UpperFunctionCriteria(left);
        BaseCriteria right = new ValueCriteria(mask);
        if (upper)
            right = new UpperFunctionCriteria(right);
        return new BinaryCriteria(left, CriteriaOperator.Like, right, escape);
    }

    /// <summary>
    /// Creates a new binary Not Like criteria containing this criteria as the left operand.
    /// The mask is a raw LIKE pattern: %, _ and [...] act as wildcards and are not escaped.
    /// For literal values see <see cref="NotContains"/>.
    /// </summary>
    /// <param name="mask">The raw like pattern.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Not Like criteria.</returns>
    public BaseCriteria NotLike(string mask, bool upper = false)
    {
        var left = this;
        if (upper)
            left = new UpperFunctionCriteria(left);
        BaseCriteria right = new ValueCriteria(mask);
        if (upper)
            right = new UpperFunctionCriteria(right);
        return new BinaryCriteria(left, CriteriaOperator.NotLike, right);
    }

    /// <summary>
    /// Creates a new binary Not Like criteria containing this criteria as the left operand,
    /// with an ESCAPE clause. The mask must already be escaped for the given escape
    /// character (see <see cref="Criteria.EscapeLikeWildcards"/>); it is used as is.
    /// For literal values prefer <see cref="NotContains"/>; for raw patterns without
    /// ESCAPE see <see cref="NotLike(string, bool)"/>.
    /// </summary>
    /// <param name="mask">The LIKE mask, already escaped for the given escape character.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <param name="escape">The LIKE escape character, appended as ESCAPE 'x'. Default is '!'.</param>
    /// <returns>A new binary Not Like criteria.</returns>
    public BaseCriteria NotLikeEscaped(string mask, bool upper = false, char escape = '!')
    {
        var left = this;
        if (upper)
            left = new UpperFunctionCriteria(left);
        BaseCriteria right = new ValueCriteria(mask);
        if (upper)
            right = new UpperFunctionCriteria(right);
        return new BinaryCriteria(left, CriteriaOperator.NotLike, right, escape);
    }

    /// <summary>
    /// Creates a new binary Starts With (LIKE '...%') criteria containing this criteria as the left operand.
    /// The text is treated as a literal value: LIKE special characters (%, _, [) in it are
    /// escaped and an ESCAPE '!' clause is appended. For intentional wildcards use
    /// <see cref="Like(string, bool)"/> instead.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Starts With criteria.</returns>
    /// <exception cref="ArgumentNullException">text is null</exception>
    public BaseCriteria StartsWith(string text, bool upper = false)
    {
        ArgumentNullException.ThrowIfNull(text);

        return LikeEscaped(Criteria.EscapeLikeWildcards(text) + "%", upper);
    }

    /// <summary>
    /// Creates a new binary Ends With (LIKE '%...') criteria containing this criteria as the left operand.
    /// The text is treated as a literal value: LIKE special characters (%, _, [) in it are
    /// escaped and an ESCAPE '!' clause is appended. For intentional wildcards use
    /// <see cref="Like(string, bool)"/> instead.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Ends With criteria.</returns>
    /// <exception cref="ArgumentNullException">text is null</exception>
    public BaseCriteria EndsWith(string text, bool upper = false)
    {
        ArgumentNullException.ThrowIfNull(text);

        return LikeEscaped("%" + Criteria.EscapeLikeWildcards(text), upper);
    }

    /// <summary>
    /// Creates a new binary Contains criteria (LIKE '%...%') containing this criteria as the left operand.
    /// The text is treated as a literal value: LIKE special characters (%, _, [) in it are
    /// escaped and an ESCAPE '!' clause is appended. For intentional wildcards use
    /// <see cref="Like(string, bool)"/> instead.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Contains criteria.</returns>
    /// <exception cref="ArgumentNullException">text is null</exception>
    public BaseCriteria Contains(string text, bool upper = false)
    {
        ArgumentNullException.ThrowIfNull(text);

        return LikeEscaped("%" + Criteria.EscapeLikeWildcards(text) + "%", upper);
    }

    /// <summary>
    /// Creates a new binary Not Contains criteria (NOT LIKE '%...%') containing this criteria as the left operand.
    /// The text is treated as a literal value: LIKE special characters (%, _, [) in it are
    /// escaped and an ESCAPE '!' clause is appended. For intentional wildcards use
    /// <see cref="NotLike(string, bool)"/> instead.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper"><c>true</c> to use the UPPER function on both sides.</param>
    /// <returns>A new binary Not Contains criteria.</returns>
    /// <exception cref="ArgumentNullException">text is null</exception>
    public BaseCriteria NotContains(string text, bool upper = false)
    {
        ArgumentNullException.ThrowIfNull(text);

        return NotLikeEscaped("%" + Criteria.EscapeLikeWildcards(text) + "%", upper);
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <typeparam name="T">The type of values.</typeparam>
    /// <param name="values">The values.</param>
    /// <returns>A new binary IN criteria.</returns>
    /// <exception cref="ArgumentNullException">values</exception>
    public BaseCriteria In<T>(params T[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException(nameof(values));

        if (values.Length == 1 &&
            values[0] is BaseCriteria bc)
        {
            return In(bc);
        }

        if (values.Length == 1 &&
            values[0] is not string &&
            values[0] is IEnumerable)
        {
            return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values[0]));
        }

        if (values.Length == 1 &&
            values[0] is ISqlQuery q)
        {
            return In(q);
        }

        return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values));
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>A new binary IN criteria.</returns>
    /// <exception cref="ArgumentNullException">statement is null or empty</exception>
    /// <remarks>
    /// The statement criteria is used as is, without adding parentheses around it.
    /// Use the <see cref="In(ISqlQuery)"/> overload for subqueries, which wraps
    /// the query in parentheses, or include them in the criteria expression.
    /// </remarks>
    public BaseCriteria In(BaseCriteria statement)
    {
        if (statement is null || statement.IsEmpty)
            throw new ArgumentNullException(nameof(statement));

        return new BinaryCriteria(this, CriteriaOperator.In, statement);
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>A new binary IN criteria.</returns>
    public BaseCriteria InStatement(BaseCriteria statement)
    {
        return In(statement);
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement query.</param>
    /// <returns>A new binary IN criteria.</returns>
    /// <exception cref="ArgumentNullException">statement is null</exception>
    /// <exception cref="InvalidOperationException">statement is an independent
    /// query with auto parameters.</exception>
    /// <remarks>
    /// Subqueries created via <see cref="SqlQuery.SubQuery()"/> already enclose
    /// themselves in parenthesis while rendering, so the statement is only
    /// wrapped in parenthesis when the query would render without them.
    /// The statement must share the same root query that this criteria will be
    /// used in (usually built via that query's SubQuery() method). An unrelated
    /// query's parameters are not available to the outer query, so embedding it
    /// may produce completely invalid results. Independent queries without
    /// parameters are safe to embed.
    /// </remarks>
    public BaseCriteria In(ISqlQuery statement)
    {
        ArgumentNullException.ThrowIfNull(statement);

        // Rendered first: auto parameters only materialize at render time, so a
        // freshly built subquery reports no autos until this ToString runs.
        var text = statement.Parent != null && !statement.OmitParens
            ? statement.ToString()!
            : "(" + statement + ")";

        statement.ThrowIfUnsharedSubQueryWithAutoParams();

        return new BinaryCriteria(this, CriteriaOperator.In, new Criteria(text));
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <typeparam name="T">The type of values.</typeparam>
    /// <param name="values">The values.</param>
    /// <returns>A new binary NOT IN criteria.</returns>
    /// <exception cref="ArgumentNullException">values is null or zero length array</exception>
    public BaseCriteria NotIn<T>(params T[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException(nameof(values));

        if (values.Length == 1 &&
            values[0] is BaseCriteria bc)
        {
            return NotIn(bc);
        }

        if (values.Length == 1 &&
            values[0] is not string &&
            values[0] is IEnumerable)
        {
            return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values[0]));
        }

        if (values.Length == 1 &&
            values[0] is ISqlQuery q)
        {
            return NotIn(q);
        }

        return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values));
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>A new binary NOT IN criteria.</returns>
    /// <exception cref="ArgumentNullException">statement is null or empty</exception>
    /// <remarks>
    /// The statement criteria is used as is, without adding parentheses around it.
    /// Use the <see cref="NotIn(ISqlQuery)"/> overload for subqueries, which wraps
    /// the query in parentheses, or include them in the criteria expression.
    /// </remarks>
    public BaseCriteria NotIn(BaseCriteria statement)
    {
        if (statement is null || statement.IsEmpty)
            throw new ArgumentNullException(nameof(statement));

        return new BinaryCriteria(this, CriteriaOperator.NotIn, statement);
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement query.</param>
    /// <returns>A new binary NOT IN criteria.</returns>
    /// <exception cref="ArgumentNullException">statement is null</exception>
    /// <exception cref="InvalidOperationException">statement is an independent
    /// query with auto parameters.</exception>
    /// <remarks>
    /// Subqueries created via <see cref="SqlQuery.SubQuery()"/> already enclose
    /// themselves in parenthesis while rendering, so the statement is only
    /// wrapped in parenthesis when the query would render without them.
    /// The statement must share the same root query that this criteria will be
    /// used in (usually built via that query's SubQuery() method). An unrelated
    /// query's parameters are not available to the outer query, so embedding it
    /// may produce completely invalid results. Independent queries without
    /// parameters are safe to embed.
    /// </remarks>
    public BaseCriteria NotIn(ISqlQuery statement)
    {
        ArgumentNullException.ThrowIfNull(statement);

        // Rendered first: auto parameters only materialize at render time, so a
        // freshly built subquery reports no autos until this ToString runs.
        var text = statement.Parent != null && !statement.OmitParens
            ? statement.ToString()!
            : "(" + statement + ")";

        statement.ThrowIfUnsharedSubQueryWithAutoParams();

        return new BinaryCriteria(this, CriteriaOperator.NotIn, new Criteria(text));
    }

    /// <summary>
    /// Implements the operator !.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// Builds SQL NOT criteria; it does not evaluate the criteria as a Boolean value.
    /// </remarks>
    public static BaseCriteria operator !(BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        return new UnaryCriteria(CriteriaOperator.Not, criteria);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, criteria2);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// A null value is intentionally not mapped to IS NULL: it renders as a
    /// NULL parameter ("= NULL", i.e. SQL UNKNOWN). Use IsNull() for null checks.
    /// </remarks>
    public static BaseCriteria operator ==(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value. Can be null (as <see cref="Enum"/> is a reference type).</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// A null value is intentionally not mapped to IS NULL: it renders as a
    /// NULL parameter ("= NULL", i.e. SQL UNKNOWN). Use IsNull() for null checks.
    /// </remarks>
    public static BaseCriteria operator ==(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.EQ, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, criteria2);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// A null value is intentionally not mapped to IS NOT NULL: it renders as
    /// a NULL parameter ("&lt;&gt; NULL", i.e. SQL UNKNOWN). Use IsNotNull() for null checks.
    /// </remarks>
    public static BaseCriteria operator !=(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value. Can be null (as <see cref="Enum"/> is a reference type).</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// A null value is intentionally not mapped to IS NOT NULL: it renders as
    /// a NULL parameter ("&lt;&gt; NULL", i.e. SQL UNKNOWN). Use IsNotNull() for null checks.
    /// </remarks>
    public static BaseCriteria operator !=(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.NE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, criteria2);
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, criteria2);
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.GE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, criteria2);
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LT, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, BaseCriteria criteria2)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, criteria2);
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, Parameter param)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ParamCriteria(param.Name));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, int value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, long value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, string value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, double value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, decimal value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, DateTime value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, Guid value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(BaseCriteria criteria1, Enum value)
    {
        return new BinaryCriteria(criteria1, CriteriaOperator.LE, new ValueCriteria(value));
    }

    private static BaseCriteria? JoinIf(BaseCriteria? criteria1, BaseCriteria? criteria2, CriteriaOperator op)
    {
        if (criteria1 is null || criteria1.IsEmpty)
            return criteria2;

        if (criteria2 is null || criteria2.IsEmpty)
            return criteria1;

        return new BinaryCriteria(criteria1, op, criteria2);
    }

    /// <summary>
    /// Implements the operator &amp;.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    [return: NotNullIfNotNull(nameof(criteria1))]
    [return: NotNullIfNotNull(nameof(criteria2))]
    public static BaseCriteria? operator &(BaseCriteria? criteria1, BaseCriteria? criteria2)
    {
        return JoinIf(criteria1, criteria2, CriteriaOperator.AND);
    }

    /// <summary>
    /// Implements the operator |.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    [return: NotNullIfNotNull(nameof(criteria1))]
    [return: NotNullIfNotNull(nameof(criteria2))]
    public static BaseCriteria? operator |(BaseCriteria? criteria1, BaseCriteria? criteria2)
    {
        return JoinIf(criteria1, criteria2, CriteriaOperator.OR);
    }

    /// <summary>
    /// Implements the operator ^.
    /// </summary>
    /// <param name="criteria1">The criteria1.</param>
    /// <param name="criteria2">The criteria2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    [return: NotNullIfNotNull(nameof(criteria1))]
    [return: NotNullIfNotNull(nameof(criteria2))]
    public static BaseCriteria? operator ^(BaseCriteria? criteria1, BaseCriteria? criteria2)
    {
        return JoinIf(criteria1, criteria2, CriteriaOperator.XOR);
    }

    /// <summary>
    /// Implements the operator ~.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <remarks>
    /// Adds parentheses to a criteria expression; it does not perform Boolean negation.
    /// </remarks>
    public static BaseCriteria operator ~(BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (!criteria.IsEmpty)
            return new UnaryCriteria(CriteriaOperator.Paren, criteria);
        return criteria;
    }

    /// <summary>
    /// Returns false so that <c>||</c> evaluates its right operand and combines
    /// both criteria with <see cref="operator |(BaseCriteria?, BaseCriteria?)"/>.
    /// </summary>
    /// <remarks>
    /// Criteria are SQL expression builders, not Boolean values. This operator
    /// always returns false; do not use a criteria in a Boolean conditional.
    /// </remarks>
#pragma warning disable IDE0060 // Remove unused parameter
    public static bool operator false(BaseCriteria statement)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return false;
    }

    /// <summary>
    /// Returns false so that <c>&amp;&amp;</c> evaluates its right operand and combines
    /// both criteria with <see cref="operator &amp;(BaseCriteria?, BaseCriteria?)"/>.
    /// </summary>
    /// <remarks>
    /// Criteria are SQL expression builders, not Boolean values. This operator
    /// always returns false; do not use a criteria in a Boolean conditional.
    /// </remarks>
#pragma warning disable IDE0060 // Remove unused parameter
    public static bool operator true(BaseCriteria statement)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return false;
    }

    /// <summary>
    /// Must override this or will get operator overload warning.
    /// </summary>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Must override this or will get operator overload warning.
    /// </summary>
    /// <param name="obj">object</param>
    /// <returns>True if equals to object</returns>
    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Converts the criteria to a string while ignoring its params, if any.
    /// <see cref="ToString()"/> raises an exception if a criteria has params, while this does not.
    /// </summary>
    /// <returns>The string representation of the criteria.</returns>
    public string ToStringIgnoreParams()
    {
        // Fresh instance per render: output is deterministic (same criteria
        // always renders the same @pN names), which the debugger display and
        // SqlQuery.Join's duplicate detection rely on. No shared state, so no
        // atomics are needed despite concurrent renders.
        return ToString(new IgnoreParams());
    }

    /// <summary>
    /// Converts the criteria to a string while ignoring its params, if any,
    /// also reporting how many auto parameters the render generated.
    /// </summary>
    /// <param name="autoParamCount">The number of auto parameters generated
    /// while rendering. When this is greater than zero the rendered text
    /// contains deterministic @pN placeholders instead of the actual values,
    /// so two renders cannot be trusted to be equal just because their text
    /// matches. Manually named parameters (ParamCriteria) don't affect this
    /// count, as they render literally and compare reliably.</param>
    /// <returns>The string representation of the criteria.</returns>
    internal string ToStringIgnoreParams(out int autoParamCount)
    {
        var ignoreParams = new IgnoreParams();
        var sb = new StringBuilder(256);
        ToString(sb, ignoreParams);
        autoParamCount = ignoreParams.AutoParamCount;
        return sb.ToString();
    }

    /// <summary>
    /// Converts the criteria to string representation while adding params to the target query.
    /// </summary>
    /// <param name="query">The target query to add params to.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public string ToString(IQueryWithParams query)
    {
        var sb = new StringBuilder(256);
        ToString(sb, query);
        return sb.ToString();
    }

    /// <summary>
    /// Converts the criteria to string. Raises an exception if
    /// criteria contains parameters.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return ToString(noParamsChecker);
    }

    /// <summary>
    /// Converts the criteria to string representation into a string builder, while adding
    /// its params to the target query.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params to.</param>
    public virtual void ToString(StringBuilder sb, IQueryWithParams query)
    {
        throw new NotImplementedException();
    }

    private class NoParamsChecker : IQueryWithParams
    {
        public bool HasAutoParams => false;

        public IQueryWithParams? Parent => null;

        public void AddParam(string name, object? value)
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public void SetParam(string name, object? value)
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public Parameter AutoParam()
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public bool IsParamsFrozen => false;

        public bool IsFrozen => false;

        public void Freeze()
        {
        }

        public void FreezeParams()
        {
        }

        public IReadOnlyDictionary<string, object?>? Params => null;

        public ISqlDialect Dialect => SqlSettings.DefaultDialect;
    }

    private class IgnoreParams : IQueryWithParams
    {
        private int next;

        internal int AutoParamCount => next;

        public bool HasAutoParams => next > 0;

        public IQueryWithParams? Parent => null;

        public void AddParam(string name, object? value)
        {
        }

        public void SetParam(string name, object? value)
        {
        }

        public Parameter AutoParam()
        {
            return new Parameter(next++.IndexParam());
        }

        public bool IsParamsFrozen => false;

        public bool IsFrozen => false;

        public void Freeze()
        {
        }

        public void FreezeParams()
        {
        }

        public IReadOnlyDictionary<string, object?>? Params => null;

        public ISqlDialect Dialect => SqlSettings.DefaultDialect;
    }
}
