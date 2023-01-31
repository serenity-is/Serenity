namespace Serenity.Data.Mapping;

/// <summary>
/// DatePart expression attribute
/// </summary>
public class DateDiffAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="part">Datepart like "year", "month" etc.</param>
    /// <param name="start">An expression that returns a date value</param>
    /// <param name="end">An expression that returns a date value</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public DateDiffAttribute(DateParts part, string start, string end)
    {
        Part = part;
        Start = start ?? throw new ArgumentNullException(nameof(start));
        End = end ?? throw new ArgumentNullException(nameof(end));
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect sqlDialect)
    {
        return "DATEDIFF(" + Part.GetName() + ", " + Start + ", " + End + ")";
    }

    /// <summary>
    /// Date part
    /// </summary>
    public DateParts Part { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public string Start { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public string End { get; }
}