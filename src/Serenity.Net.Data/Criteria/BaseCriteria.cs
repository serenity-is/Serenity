using System.Collections;
using System.Diagnostics;

namespace Serenity.Data;

/// <summary>
/// Base criteria object type
/// </summary>
/// <seealso cref="ICriteria" />
[DebuggerDisplay("{ToStringIgnoreParams()}")]
[JsonConverter(typeof(JsonCriteriaConverter))]
public abstract class BaseCriteria : ICriteria
{
    private static readonly NoParamsChecker noParamsChecker = new NoParamsChecker();
    private static readonly IgnoreParams ignoreParams = new IgnoreParams();

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
    /// <returns></returns>
    public BaseCriteria IsNull()
    {
        return new UnaryCriteria(CriteriaOperator.IsNull, this);
    }

    /// <summary> 
    /// Creates a new unary IsNotNull criteria containing this criteria as the operand.
    /// </summary>
    /// <returns></returns>
    public BaseCriteria IsNotNull()
    {
        return new UnaryCriteria(CriteriaOperator.IsNotNull, this);
    }

    /// <summary>
    /// Creates a new binary Like criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The LIKE mask.</param>
    /// <returns></returns>
    public BaseCriteria Like(string mask)
    {
        return new BinaryCriteria(this, CriteriaOperator.Like, new ValueCriteria(mask));
    }

    /// <summary>
    /// Creates a new binary Not Like criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The like mask.</param>
    /// <returns></returns>
    public BaseCriteria NotLike(string mask)
    {
        return new BinaryCriteria(this, CriteriaOperator.NotLike, new ValueCriteria(mask));
    }

    /// <summary>
    /// Creates a new binary Stars With (LIKE '...%') criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The starts with mask.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">mask is null</exception>
    public BaseCriteria StartsWith(string mask)
    {
        if (mask == null)
            throw new ArgumentNullException("mask");

        return Like(mask + "%");
    }

    /// <summary>
    /// Creates a new binary Ends With (LIKE '%...') criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The ends with mask.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">mask is null</exception>
    public BaseCriteria EndsWith(string mask)
    {
        if (mask == null)
            throw new ArgumentNullException("mask");

        return Like("%" + mask);
    }

    /// <summary>
    /// Creates a new binary Contains criteria (LIKE '%...%') containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The contains mask.</param>
    /// <returns></returns>
    public BaseCriteria Contains(string mask)
    {
        return Like("%" + mask + "%");
    }

    /// <summary>
    /// Creates a new binary Not Contains criteria (NOT LIKE '%...%') containing this criteria as the left operand.
    /// </summary>
    /// <param name="mask">The contains mask.</param>
    /// <returns></returns>
    public BaseCriteria NotContains(string mask)
    {
        return NotLike("%" + mask + "%");
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <typeparam name="T">Type of values</typeparam>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">values</exception>
    public BaseCriteria In<T>(params T[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException("values");

        if (values.Length == 1 &&
            values[0] is BaseCriteria)
        {
            return In((BaseCriteria)(object)values[0]);
        }

        if (values.Length == 1 &&
            values[0] is not string &&
            values[0] is IEnumerable)
        {
            return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values[0]));
        }

        if (values.Length == 1 &&
            values[0] is ISqlQuery)
        {
            return In((ISqlQuery)(object)values[0]);
        }

        return new BinaryCriteria(this, CriteriaOperator.In, new ValueCriteria(values));
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">statement is null or empty</exception>
    public BaseCriteria In(BaseCriteria statement)
    {
        if (statement is null || statement.IsEmpty)
            throw new ArgumentNullException("statement");

        return new BinaryCriteria(this, CriteriaOperator.In, statement);
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    public BaseCriteria InStatement(BaseCriteria statement)
    {
        return In(statement);
    }

    /// <summary>
    /// Creates a new binary IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement query.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">statement is null</exception>
    public BaseCriteria In(ISqlQuery statement)
    {
        if (statement is null)
            throw new ArgumentNullException("statement");

        return new BinaryCriteria(this, CriteriaOperator.In, new Criteria(statement));
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <typeparam name="T">Type of values</typeparam>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">values is null or zero length array</exception>
    public BaseCriteria NotIn<T>(params T[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException("values");

        if (values.Length == 1 &&
            values[0] is BaseCriteria)
        {
            return NotIn((BaseCriteria)(object)values[0]);
        }

        if (values.Length == 1 &&
            values[0] is not string &&
            values[0] is IEnumerable)
        {
            return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values[0]));
        }

        if (values.Length == 1 &&
            values[0] is ISqlQuery)
        {
            return NotIn((ISqlQuery)(object)values[0]);
        }

        return new BinaryCriteria(this, CriteriaOperator.NotIn, new ValueCriteria(values));
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">statement is null or empty</exception>
    public BaseCriteria NotIn(BaseCriteria statement)
    {
        if (statement is null || statement.IsEmpty)
            throw new ArgumentNullException("statement");

        return new BinaryCriteria(this, CriteriaOperator.NotIn, statement);
    }

    /// <summary>
    /// Creates a new binary NOT IN criteria containing this criteria as the left operand.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">statement is null</exception>
    public BaseCriteria NotIn(ISqlQuery statement)
    {
        if (statement is null)
            throw new ArgumentNullException("statement");

        return new BinaryCriteria(this, CriteriaOperator.NotIn, new Criteria(statement));
    }

    /// <summary>
    /// Implements the operator !.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !(BaseCriteria criteria)
    {
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
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
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
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
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

    private static BaseCriteria JoinIf(BaseCriteria criteria1, BaseCriteria criteria2, CriteriaOperator op)
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
    public static BaseCriteria operator &(BaseCriteria criteria1, BaseCriteria criteria2)
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
    public static BaseCriteria operator |(BaseCriteria criteria1, BaseCriteria criteria2)
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
    public static BaseCriteria operator ^(BaseCriteria criteria1, BaseCriteria criteria2)
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
    public static BaseCriteria operator ~(BaseCriteria criteria)
    {
        if (!criteria.IsEmpty)
            return new UnaryCriteria(CriteriaOperator.Paren, criteria);
        return criteria;
    }

    /// <summary>
    /// Must return FALSE from this for short circuit OR (||) to return 
    /// a new binary criteria merging left and right operands in any case
    /// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
    public static bool operator false(BaseCriteria statement)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return false;
    }

    /// <summary>
    /// Must ALSO return FALSE from this for short circuit AND (&amp;&amp;) to return 
    /// a new binary criteria merging left and right operands in any case
    /// https://msdn.microsoft.com/en-us/library/aa691312
    /// </summary>
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
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Converts the criteria to string while ignoring its params if any.
    /// ToString() raises an exception if a criteria has params, while this not.
    /// </summary>
    /// <returns></returns>
    public string ToStringIgnoreParams()
    {
        return ToString(ignoreParams);
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
        public void AddParam(string name, object value)
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public void SetParam(string name, object value)
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public Parameter AutoParam()
        {
            throw new InvalidOperationException("Criteria should not have parameters!");
        }

        public IDictionary<string, object> Params => null;

        public ISqlDialect Dialect => SqlSettings.DefaultDialect;
    }

    private class IgnoreParams : IQueryWithParams
    {
        private static int next;

        public void AddParam(string name, object value)
        {
        }

        public void SetParam(string name, object value)
        {
        }

        public Parameter AutoParam()
        {
            return new Parameter((next++).IndexParam());
        }

        public IDictionary<string, object> Params => null;

        public ISqlDialect Dialect => SqlSettings.DefaultDialect;
    }
}
