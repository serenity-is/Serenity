namespace Serenity.Data.Mapping;

/// <summary>
/// Concat expression attribute
/// </summary>
public class ConcatAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="expression1">SQL Expression 1</param>
    /// <param name="expression2">SQL Expression 2</param>
    /// <param name="rest">Additional expressions</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public ConcatAttribute(object expression1, object expression2, params object[] rest)
    {
        if (rest == null)
            throw new ArgumentNullException(nameof(rest));

        if (rest.Length > 0)
        {
            Expressions = new[] {
                expression1 ?? throw new ArgumentNullException(nameof(expression1)),
                expression2 ?? throw new ArgumentNullException(nameof(expression2)),
                rest[0] ?? throw new ArgumentNullException(nameof(rest))
            };

            if (rest.Length > 1)
                Expressions = Expressions.Concat(rest.Skip(1).Select(
                    x => x ?? throw new ArgumentNullException(nameof(rest)))).ToArray();
        }
        else
        {
            Expressions = new[] {
                expression1 ?? throw new ArgumentNullException(nameof(expression1)),
                expression2 ?? throw new ArgumentNullException(nameof(expression2))
            };
        }
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        string coalesce(string s)
        {
            if (NullAsEmpty && s != "''" && s != "' '")
                return "COALESCE(" + s + ", '')";

            return s;
        }

        var expressions = Expressions.Select(x => ToString(x, dialect)).ToArray();

        if (dialect.CanUseConcat)
        {
            switch (dialect?.ServerType)
            {
                case nameof(ServerType.SqlServer):
                case nameof(ServerType.Postgres):
                    // SqlServer and Postgres never return null from CONCAT even when all args are null
                    return "CONCAT(" + string.Join(", ", expressions) + ")";

                case nameof(ServerType.Oracle):
                    // Oracle accepts only two arguments in CONCAT function 
                    // and it only returns null when both arguments are null
                    var result = coalesce("CONCAT(" + expressions[0] + ", " 
                        + expressions[1] + ")");

                    for (var i = 2; i < expressions.Length; i++)
                        result = "CONCAT(" + result + ", " + expressions[i] + ")";

                    return result;

                default:
                    return "CONCAT(" + string.Join(", ", expressions.Select(coalesce)) + ")";
            }
        }
        else
        {
            return (dialect?.ServerType) switch
            {
                nameof(ServerType.SqlServer) => "(" + string.Join(" + ", expressions.Select(coalesce)) + ")",
                _ => "(" + string.Join(" || ", expressions.Select(coalesce)) + ")"
            };
        }
    }

    /// <summary>
    /// Gets the expressions
    /// </summary>
    public object[] Expressions { get; }

    /// <summary>
    /// When true (default), NULLS values are assumed to be empty.
    /// This is done using the COALESCE operator for dialects
    /// that is necessary. SQLServer Concat() returns empty string
    /// even when all the arguments are null. Oracle only returns null
    /// when all the arguments are null.
    /// </summary>
    public bool NullAsEmpty { get; set; } = true;
}

/// <summary>
/// Please use <see cref="ConcatAttribute" />
/// </summary>
[Obsolete("Use ConcatAttribute")]
public class ConcatExpressionAttribute : ConcatAttribute
{
    /// <inheritdoc/>
    public ConcatExpressionAttribute(string expression1, string expression2, params object[] rest)
        : base(expression1, expression2, rest)
    {
    }
}