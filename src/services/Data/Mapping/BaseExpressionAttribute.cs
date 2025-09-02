
namespace Serenity.Data.Mapping;

/// <summary>
/// Specifies SQL expression this property corresponds to.
/// You may use brackets ([]) to escape identifiers. Brackets will be converted to database specific quotes.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class BaseExpressionAttribute : Attribute
{
    /// <summary>
    /// Gets the expression for specified dialect. Prefer ToString(ISqlDialect)
    /// version as it allows the dialect to optionally customize the formatted 
    /// expression via ISqlExpressionTranslator interface.
    /// </summary>
    /// <param name="dialect">Target dialect</param>
    public abstract string Translate(ISqlDialect dialect);

    /// <summary>
    /// Converts the expression to string for specified dialect
    /// </summary>
    /// <param name="dialect">Target dialect</param>
    public string ToString(ISqlDialect dialect)
    {
        string expression;
        if (dialect is ISqlExpressionTranslator translator)
        {
            expression = translator.Translate(this) ??
                Translate(dialect);
        }
        else
            expression = Translate(dialect);

        if (string.IsNullOrEmpty(Format))
            return expression;

        return string.Format(Format, expression);
    }

    /// <summary>
    /// Gets sets an optional format string to apply with
    /// {0} placeholder for the expression.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Convert the expression to string. Used by derived expression attributes
    /// to convert their constructor arguments to string while supporting
    /// other expression attribute types, or a special array with the first
    /// argument as the attribute type and others as its constructor parameters.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="dialect">Target dialect</param>
    /// <exception cref="ArgumentNullException">Dialect is null</exception>
    public static string ToString(object expression, ISqlDialect dialect)
    {
        if (dialect is null)
            throw new ArgumentNullException(nameof(dialect));

        if (expression is null)
            return SqlConversions.Null;

        if (expression is string s)
            return s;

        if (expression is int i)
            return SqlConversions.ToSql(i);

        if (expression is bool b)
            return SqlConversions.ToSql(b);

        if (expression is double d)
            return SqlConversions.ToSql(d);

        if (expression is decimal m)
            return SqlConversions.ToSql(m);

        if (expression is long l)
            return SqlConversions.ToSql(l);

        if (expression is DateTime dt)
            return SqlConversions.ToSql(dt, dialect);

        if (expression is Type t1 &&
            !t1.IsAbstract &&
            typeof(BaseExpressionAttribute).IsAssignableFrom(t1))
        {
            var instance = (BaseExpressionAttribute)(Activator.CreateInstance(t1));
            return instance.ToString(dialect);
        }

        if (expression is IEnumerable<object> enumerable &&
            enumerable.FirstOrDefault() is Type t2 &&
            !t2.IsAbstract &&
            typeof(BaseExpressionAttribute).IsAssignableFrom(t2))
        {
            var args = enumerable.Skip(1).ToArray();
            var instance = (BaseExpressionAttribute)Activator.CreateInstance(t2, args);
            return instance.ToString(dialect);
        }

        return Convert.ToString(expression, CultureInfo.InvariantCulture);
    }
}