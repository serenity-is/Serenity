
namespace Serenity.Data;

/// <summary>
/// Interface for types that can convert an <see cref="ISqlQuery"/> to its string representation.
/// </summary>
public interface ISqlQueryToString
{
    /// <summary>
    /// Converts the query to string.
    /// </summary>
    /// <param name="sqlQuery">The SQL query to convert.</param>
    /// <returns>The string representation of the query.</returns>
    string ToString(ISqlQuery sqlQuery);
}
