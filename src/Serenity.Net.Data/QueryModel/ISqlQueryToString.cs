
namespace Serenity.Data;

/// <summary>
/// SqlQuery interface.
/// </summary>
/// <seealso cref="IQueryWithParams" />
/// <seealso cref="IChainable" />
public interface ISqlQueryToString
{
    /// <summary>
    /// Converts the query to string
    /// </summary>
    /// <param name="sqlQuery"></param>
    /// <returns></returns>
    string ToString(ISqlQuery sqlQuery);
}
