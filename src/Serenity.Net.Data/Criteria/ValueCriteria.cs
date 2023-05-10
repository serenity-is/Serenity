using System.Collections;

namespace Serenity.Data;

/// <summary>
/// Criteria object with one value
/// </summary>
/// <seealso cref="BaseCriteria" />
public class ValueCriteria : BaseCriteria
{
    private readonly object value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueCriteria"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ValueCriteria(object value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value => value;

    /// <summary>
    /// Converts the criteria to string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params to.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        if (value is IEnumerable enumerable && value is not string)
        {
            var c = 0;
            foreach (var k in enumerable)
                c++;

            int i = 0;
            sb.Append('(');
            foreach (var k in enumerable)
            {
                if (i++ > 0)
                    sb.Append(',');

                if (c > 10)
                {
                    if (IsIntegerType(k))
                    {
                        sb.Append(k.ToString());
                        continue;
                    }
                    else if (k is Enum)
                    {
                        sb.Append(Convert.ToInt64(k).ToString());
                        continue;
                    }
                }
                sb.Append(AddParam(query, k).Name);
            }
            sb.Append(')');
        }
        else
        {
            sb.Append(AddParam(query, value).Name);
        }
    }

    private bool IsIntegerType(object k)
    {
        if (k == null)
            return false;

        if (k is int || k is long)
            return true;

        if (!k.GetType().IsPrimitive)
            return false;

        return k is byte ||
               k is sbyte ||
               k is short ||
               k is ushort ||
               k is uint ||
               k is ulong;
    }

    private Parameter AddParam(IQueryWithParams query, object value)
    {
        var param = query.AutoParam();
        query.AddParam(param.Name, value);
        return param;
    }
}