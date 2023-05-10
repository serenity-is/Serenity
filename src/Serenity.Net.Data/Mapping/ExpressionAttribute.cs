
namespace Serenity.Data.Mapping;

/// <summary>
/// Specifies SQL expression this property corresponds to.
/// You may use brackets ([]) to escape identifiers. Brackets will be converted to database specific quotes.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ExpressionAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Specifies SQL expression this property corresponds to.
    /// </summary>
    /// <param name="value">An SQL expression like (T0.Firstname + ' ' + T0.LastName)</param>
    public ExpressionAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Specifies SQL expression and dialects this property corresponds to.
    /// </summary>
    /// <param name="expression">An SQL expression like (T0.Firstname + ' ' + T0.LastName)</param>
    /// <param name="serverTypes">Dialects like <see cref="ServerType.MySql" />, <see cref="ServerType.Sqlite" />.</param>
    public ExpressionAttribute(string expression, params ServerType[] serverTypes)
        : this(expression)
    {
        Dialect = string.Join(",", serverTypes);
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        return Value;
    }

    /// <summary>
    /// Gets the expression
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets or sets the dialect.
    /// </summary>
    /// <value>
    /// The dialect.
    /// </value>
    public string Dialect { get; set; }

    /// <summary>
    /// Gets or sets the negating of the dialect.
    /// </summary>
    /// <value>
    /// The negating of the dialect.
    /// </value>
    public bool NegateDialect
    {
        get => Dialect != null && Dialect.StartsWith('!');
        set => Dialect = value ? (!NegateDialect ? ("!" + Dialect) : Dialect) :
            (NegateDialect ?  Dialect[1..] : Dialect);
    }
}