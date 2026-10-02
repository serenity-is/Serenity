
namespace Serenity.Data;

/// <summary>
/// Interface for types that can convert an <see cref="ISqlQuery"/> to its string representation.
/// </summary>
/// <remarks>
/// Implementations are responsible for validating that the query is renderable,
/// including rejecting queries without selected columns when their SQL format requires them.
/// </remarks>
public interface ISqlQueryToString
{
    /// <summary>
    /// Converts the query to string.
    /// </summary>
    /// <param name="sqlQuery">The SQL query to convert.</param>
    /// <returns>The string representation of the query.</returns>
    string ToString(ISqlQuery sqlQuery);
}
