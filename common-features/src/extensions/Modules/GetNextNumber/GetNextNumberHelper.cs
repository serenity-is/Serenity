using System.Data;

namespace Serenity.Extensions;

public static class GetNextNumberHelper
{
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