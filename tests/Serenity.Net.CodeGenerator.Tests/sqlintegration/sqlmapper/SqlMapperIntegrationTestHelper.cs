namespace Serenity.Data;

internal static class SqlMapperIntegrationTestHelper
{
    public static void AssertParameterizedQueryWorks(string provider, ISqlDialect dialect, string? from = null)
    {
        using var actualConnection = SqlIntegrationConnections.CreateConnection(provider);
        using var connection = new WrappedConnection(actualConnection, dialect);

        var query = new SqlQuery().Select("1");
        if (!string.IsNullOrEmpty(from))
            query.From(from);

        query.Where(new Criteria("1") == 1)
            .Where(new Criteria("'serenity'") == "serenity");

        Assert.Contains("@p1", query.Params!.Keys);
        Assert.Contains("@p2", query.Params.Keys);

        var translatedSql = SqlConversions.Translate(query.ToString()!, connection);
        Assert.Contains(dialect.ParameterPrefix + "p1", translatedSql, StringComparison.Ordinal);
        Assert.Contains(dialect.ParameterPrefix + "p2", translatedSql, StringComparison.Ordinal);

        Assert.Equal(1, connection.Query<int>(query).Single());
    }
}
