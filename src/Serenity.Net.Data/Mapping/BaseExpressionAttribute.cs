
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
        if (dialect is ISqlExpressionTranslator translator)
        {
            var expression = translator.Translate(this);
            if (expression != null)
                return expression;
        }

        return Translate(dialect);
    }
}