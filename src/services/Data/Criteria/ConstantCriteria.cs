namespace Serenity.Data;

/// <summary>
/// A constant criteria object, which only contains a value expression
/// that would be converted to its string representation in SQL,
/// not a parameterized value.
/// </summary>
/// <seealso cref="Criteria" />
public class ConstantCriteria : Criteria
{
    private readonly string?[]? stringValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ConstantCriteria(int value)
        : base(value.ToInvariant())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ConstantCriteria(IEnumerable<int> values)
        : base(string.Join(",", values))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ConstantCriteria(long value)
        : base(value.ToInvariant())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ConstantCriteria(IEnumerable<long> values)
        : base(string.Join(",", values))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ConstantCriteria(string value)
        : this([value])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantCriteria"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ConstantCriteria(IEnumerable<string> values)
        : this((values ?? throw new ArgumentNullException(nameof(values))).ToArray())
    {
    }

    private ConstantCriteria(string?[] values)
        : base(string.Join(",", values.Select(x => x!.ToSql())))
    {
        stringValues = values;
    }

    /// <inheritdoc/>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        if (stringValues is not null)
        {
            for (var i = 0; i < stringValues.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(stringValues[i]!.ToSql(query.Dialect));
            }

            return;
        }

        base.ToString(sb, query);
    }
}
