namespace Serenity.Data;

public abstract partial class Field
{
    /// <summary>
    /// Creates a new "the Field IS NULL" criteria.
    /// </summary>
    /// <returns>The IS NULL criteria.</returns>
    public BaseCriteria IsNull()
    {
        return Criteria.IsNull();
    }

    /// <summary>
    /// Creates a new "the Field IS NOT NULL" criteria.
    /// </summary>
    /// <returns>The IS NOT NULL criteria.</returns>
    public BaseCriteria IsNotNull()
    {
        return Criteria.IsNotNull();
    }

    /// <summary>
    /// Creates a new "the Field LIKE mask" criteria.
    /// </summary>
    /// <param name="mask">The raw LIKE mask.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <returns>The LIKE criteria.</returns>
    public BaseCriteria Like(string mask, bool upper = false)
    {
        return Criteria.Like(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field NOT LIKE mask" criteria.
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <returns>The NOT LIKE criteria.</returns>
    public BaseCriteria NotLike(string mask)
    {
        return Criteria.NotLike(mask);
    }

    /// <summary>
    /// Creates a new "the Field LIKE mask ESCAPE 'x'" criteria.
    /// The mask must already be escaped (see <see cref="Criteria.EscapeLikeWildcards"/>).
    /// </summary>
    /// <param name="mask">The LIKE mask, already escaped for the given escape character.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <param name="escape">The LIKE escape character. Default is '!'.</param>
    /// <returns>The LIKE criteria.</returns>
    public BaseCriteria LikeEscaped(string mask, bool upper = false, char escape = '!')
    {
        return Criteria.LikeEscaped(mask, upper, escape);
    }

    /// <summary>
    /// Creates a new "the Field NOT LIKE mask ESCAPE 'x'" criteria.
    /// The mask must already be escaped (see <see cref="Criteria.EscapeLikeWildcards"/>).
    /// </summary>
    /// <param name="mask">The LIKE mask, already escaped for the given escape character.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <param name="escape">The LIKE escape character. Default is '!'.</param>
    /// <returns>The NOT LIKE criteria.</returns>
    public BaseCriteria NotLikeEscaped(string mask, bool upper = false, char escape = '!')
    {
        return Criteria.NotLikeEscaped(mask, upper, escape);
    }

    /// <summary>
    /// Creates a new "the Field STARTS WITH mask" criteria.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <returns>The STARTS WITH criteria.</returns>
    public BaseCriteria StartsWith(string text, bool upper = false)
    {
        return Criteria.StartsWith(text, upper);
    }

    /// <summary>
    /// Creates a new "the Field ENDS WITH mask" criteria.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <returns>The ENDS WITH criteria.</returns>
    public BaseCriteria EndsWith(string text, bool upper = false)
    {
        return Criteria.EndsWith(text, upper);
    }

    /// <summary>
    /// Creates a new "the Field CONTAINS mask" criteria.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <returns>The CONTAINS criteria.</returns>
    public BaseCriteria Contains(string text, bool upper = false)
    {
        return Criteria.Contains(text, upper);
    }

    /// <summary>
    /// Creates a new "the Field NOT CONTAINS mask" criteria.
    /// </summary>
    /// <param name="text">The literal text to search for.</param>
    /// <param name="upper">True to use the UPPER function on both sides.</param>
    /// <returns>The NOT CONTAINS criteria.</returns>
    public BaseCriteria NotContains(string text, bool upper = false)
    {
        return Criteria.NotContains(text, upper);
    }

    /// <summary>
    /// Creates a new "the Field IN (values...)" criteria.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values.</param>
    /// <returns>The IN criteria.</returns>
    public BaseCriteria In<T>(params T[] values)
    {
        return Criteria.In(values);
    }

    /// <summary>
    /// Creates a new "the Field NOT IN (values...)" criteria.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values.</param>
    /// <returns>The NOT IN criteria.</returns>
    public BaseCriteria NotIn<T>(params T[] values)
    {
        return Criteria.NotIn(values);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == criteria;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="field">The field.</param>
    /// <returns>The result of the operator.</returns>
    public static BaseCriteria operator ==(BaseCriteria criteria, Field field)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(field);
        return criteria == field.Criteria;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria == field2.Criteria;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == param;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator ==(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria == value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != criteria;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="field">The field.</param>
    /// <returns>The result of the operator.</returns>
    public static BaseCriteria operator !=(BaseCriteria criteria, Field field)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(field);
        return criteria != field.Criteria;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria != field2.Criteria;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != param;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator !=(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria != value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < criteria;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria < field2.Criteria;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < param;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria < value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= criteria;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria <= field2.Criteria;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= param;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator <=(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria <= value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > criteria;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria > field2.Criteria;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > param;
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria > value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, BaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= criteria;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, DateTime value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, decimal value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, double value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="field2">The field2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, Field field2)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(field2);
        return field.Criteria >= field2.Criteria;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, Guid value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, int value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, long value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, Parameter param)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= param;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static BaseCriteria operator >=(Field field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.Criteria >= value;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}