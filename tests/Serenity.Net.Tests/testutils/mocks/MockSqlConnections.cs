
namespace Serenity.TestUtils;

public class MockSqlConnections : ISqlConnections
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
        return OnNewByKey?.Invoke(connectionKey) ?? throw new NotImplementedException();
    }

    public IConnectionString TryGetConnectionString(string connectionKey)
    {
        return null;
    }

    public Func<string, IDbConnection> OnNewByKey { get; set; }
}