namespace Serenity.Data.Mapping;

/// <summary>
/// Trim expression attribute that only removes the spaces at the start of a string, like LTRIM of SQL Server.
/// </summary>
public class TrimLeftAttribute : TrimAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="expression">An expression that returns a string.</param>
    /// <exception cref="ArgumentNullException">The expression is null.</exception>
    public TrimLeftAttribute(object expression)
        : base(expression)
    {
        Trailing = false;
    }
}
