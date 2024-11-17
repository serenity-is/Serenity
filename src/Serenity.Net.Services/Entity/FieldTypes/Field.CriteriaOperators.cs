namespace Serenity.Data;

public abstract partial class Field
{
    /// <summary>
    /// Creates a new "the Field IS NULL" criteria
    /// </summary>
    public BaseCriteria IsNull()
    {
        return Criteria.IsNull();
    }

    /// <summary>
    /// Creates a new "the Field IS NOT NULL" criteria
    /// </summary>
    /// <returns></returns>
    public BaseCriteria IsNotNull()
    {
        return Criteria.IsNotNull();
    }

    /// <summary>
    /// Creates a new "the Field LIKE mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <param name="upper">True to use UPPER function both sides</param>
    /// <returns></returns>
    public BaseCriteria Like(string mask, bool upper = false)
    {
        return Criteria.Like(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field NOT LIKE mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <returns></returns>
    public BaseCriteria NotLike(string mask)
    {
        return Criteria.NotLike(mask);
    }

    /// <summary>
    /// Creates a new "the Field STARTS WITH mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <param name="upper">True to use UPPER function both sides</param>
    public BaseCriteria StartsWith(string mask, bool upper = false)
    {
        return Criteria.StartsWith(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field ENDS WITH mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <param name="upper">True to use UPPER function both sides</param>
    public BaseCriteria EndsWith(string mask, bool upper = false)
    {
        return Criteria.EndsWith(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field CONTAINS mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <param name="upper">True to use UPPER function both sides</param>
    /// <returns></returns>
    public BaseCriteria Contains(string mask, bool upper = false)
    {
        return Criteria.Contains(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field NOT CONTAINS mask" criteria
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <param name="upper">True to use UPPER function both sides</param>
    public BaseCriteria NotContains(string mask, bool upper = false)
    {
        return criteria.NotContains(mask, upper);
    }

    /// <summary>
    /// Creates a new "the Field IN (values...)" criteria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    public BaseCriteria In<T>(params T[] values)
    {
        return Criteria.In(values);
    }

    /// <summary>
    /// Creates a new "the Field NOT IN (values...)" criteria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <returns></returns>
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
        return field.Criteria == criteria;
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
        return field.Criteria != criteria;
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
        return field.Criteria >= value;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
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