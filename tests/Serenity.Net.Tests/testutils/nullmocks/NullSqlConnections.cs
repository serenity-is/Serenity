
namespace Serenity.TestUtils;

public class NullSqlConnections : ISqlConnections
{
    public IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return [];
    }

    public IDbConnection New(string connectionString, string providerName, ISqlDialect dialect)
    {
        throw new NotImplementedException();
    }

    public IDbConnection NewByKey(string connectionKey)
    {
        throw new NotImplementedException();
    }

    public IConnectionString TryGetConnectionString(string connectionKey)
    {
        return null;
    }
}