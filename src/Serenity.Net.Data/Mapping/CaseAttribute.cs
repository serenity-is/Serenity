namespace Serenity.Data.Mapping;

/// <summary>
/// Concat expression attribute
/// </summary>
public class CaseAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="when1">When expression 1</param>
    /// <param name="then1">When expression 2</param>
    /// <param name="restOrElse">Additional when then pairs, followed by an optional ELSE statement.
    /// Else assumed only if the number of elements is odd</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public CaseAttribute(object when1, object then1, params object[] restOrElse)
    {
        if (restOrElse == null)
            throw new ArgumentNullException(nameof(restOrElse));

        When = new[] { when1 ?? throw new ArgumentNullException(nameof(when1)) };
        Then = new[] { then1 ?? throw new ArgumentNullException(nameof(then1)) };

        var modulo = restOrElse.Length % 2;
        if (modulo == 1)
            Else = restOrElse[^1] ?? throw new ArgumentNullException(nameof(restOrElse));

        if (restOrElse.Length > 1)
        {
            When = When.Concat(restOrElse.SkipLast(modulo).Where((x, i) => i % 2 == 0)
                .Select(x => x ?? throw new ArgumentNullException(nameof(restOrElse)))).ToArray();

            Then = Then.Concat(restOrElse.Where((x, i) => i % 2 == 1)
                .Select(x => x ?? throw new ArgumentNullException(nameof(restOrElse)))).ToArray();
        }
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        var whenElse = string.Join(" ", When.Select((x, i) =>
            $"WHEN {ToString(x, dialect)} THEN {ToString(Then[i], dialect)}"));

        return "(CASE " + (Switch != null ? (ToString(Switch, dialect) + " ") : "") + 
            whenElse + (Else != null ? (" ELSE " + ToString(Else, dialect)) : "") + " END)";
    }

    /// <summary>
    /// Gets the switch expression
    /// </summary>
    public object Switch { get; set; }

    /// <summary>
    /// Gets the when expressions
    /// </summary>
    public object[] When { get; }

    /// <summary>
    /// Gets the then expressions
    /// </summary>
    public object[] Then { get; }

    /// <summary>
    /// Gets the else expression
    /// </summary>
    public object Else { get; set; }
}
