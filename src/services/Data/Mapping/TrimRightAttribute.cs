namespace Serenity.Data.Mapping;

/// <summary>
/// Trim expression attribute that only removes the spaces at the end of a string, like RTRIM of SQL Server.
/// </summary>
public class TrimRightAttribute : TrimAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="expression">An expression that returns a string.</param>
    /// <exception cref="ArgumentNullException">The expression is null.</exception>
    public TrimRightAttribute(object expression)
        : base(expression)
    {
        Leading = false;
    }
}
