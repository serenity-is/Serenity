using System.Data;

namespace Serenity.Extensions;

/// <summary>
/// Helper for generating the next number in a sequence with a prefix.
/// </summary>
public static class GetNextNumberHelper
{
    /// <summary>
    /// Gets the next number for the specified field based on the request prefix and length.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <param name="request">The request containing the prefix and length.</param>
    /// <param name="field">The field to query for the maximum existing number.</param>
    /// <returns>The next number and its serial representation.</returns>
    public static GetNextNumberResponse GetNextNumber(IDbConnection connection, 
        GetNextNumberRequest request, Field field)
    {
        var prefix = request.Prefix ?? "";

        var max = connection.Query<string>(new SqlQuery()
            .From(field.Fields)
            .Select(Sql.Max(field.Expression))
            .Where(
                field.StartsWith(prefix) &&
                field >= prefix.PadRight(request.Length, '0') &&
                field <= prefix.PadRight(request.Length, '9')))
            .FirstOrDefault();

        var response = new GetNextNumberResponse
        {
            Number = max == null ||
            !long.TryParse(max[prefix.Length..], out long l) ? 1 : l + 1
        };

        response.Serial = prefix + response.Number.ToString(CultureInfo.InvariantCulture)
            .PadLeft(request.Length - prefix.Length, '0');

        return response;
    }
}