namespace Serenity.Data.Schema;

/// <summary>
/// Basic connectivity checks for the database providers used by schema providers.
/// Server connection strings can be supplied as SQLINTEGRATIONTEST_{SERVER}_CONNECTION.
/// Otherwise SQLINTEGRATIONTEST_{SERVER}_HOST / _PASSWORD override the shared
/// SQLINTEGRATIONTEST_HOST / _PASSWORD settings. Hosts may include a port, and SQL Server
/// hosts may include an instance (for example .\SQLExpress:1345). Defaults use common
/// local Docker ports: SQL Server 1433, PostgreSQL 5432, MySQL 3306, Oracle 1521, and
/// Firebird 3050. Defaults match the compose users/databases (SQL Server sa, PostgreSQL root,
/// MySQL root, Oracle root, and Firebird SYSDBA), Oracle XEPDB1 service, and Firebird
/// /var/lib/firebird/data/test.fdb database; customize with a full connection string if needed.
/// </summary>
[Trait("tag", "sqldb")]
public partial class SchemaProviderIntegrationTests_BasicConnectivity
{
    [Theory]
    [InlineData("SqlServer")]
    [InlineData("Postgres")]
    [InlineData("MySql")]
    [InlineData("Oracle")]
    [InlineData("Firebird")]
    [InlineData("Sqlite")]
    public void ProviderConnection_Opens_And_Closes(string provider)
    {
        if (SqlIntegrationConnections.ShouldSkip(provider))
            return;

        using var connection = SqlIntegrationConnections.CreateConnection(provider);

        connection.Open();
        Assert.Equal(ConnectionState.Open, connection.State);

        connection.Close();
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

}
