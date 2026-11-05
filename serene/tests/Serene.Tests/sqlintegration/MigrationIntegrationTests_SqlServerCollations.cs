using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests
{
    private async Task RunOnSqlServerWithOptions(string collation, string comparer, bool partialContainment,
        string expectedCollation)
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
        using var serverConnection = SqlIntegrationConnections.CreateConnection("SqlServer");

        testCleanup += () =>
        {
            SqlConnection.ClearAllPools();
            using var serverConnection = SqlIntegrationConnections.CreateConnection("SqlServer");
            serverConnection.Open();
            foreach (var database in new[] { defaultDatabase, northwindDatabase })
                SqlIntegrationConnections.ExecuteSql(serverConnection,
                    $"IF DB_ID(N'{database}') IS NOT NULL BEGIN ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{database}]; END");
        };

        await RunMigrationsAndServiceCall(WithSqlServerDatabase(serverConnection.ConnectionString, defaultDatabase),
            WithSqlServerDatabase(serverConnection.ConnectionString, northwindDatabase), "Microsoft.Data.SqlClient",
            collation, comparer, partialContainment, expectedCollation);
    }

    [Fact]
    public async Task SqlServer_AutoMap_Creates_Database_With_Collation_Of_Comparer()
    {
        if (SqlIntegrationConnections.ShouldSkip("SqlServer"))
            return;

        await RunOnSqlServerWithOptions("automap", "tr-TR:IgnoreCase", false, "Turkish_100_CI_AS");
    }

    /// <summary>
    /// This needs the "contained database authentication" setting of the server, which
    /// is not enabled by default, so it only runs when explicitly asked for.
    /// </summary>
    [Fact]
    public async Task SqlServer_PartialContainment_Allows_Case_Sensitive_Database_Collation()
    {
        if (SqlIntegrationConnections.ShouldSkip("SqlServer") ||
            Environment.GetEnvironmentVariable("SQLINTEGRATIONTEST_SQLSERVER_CONTAINED") != "true")
            return;

        await RunOnSqlServerWithOptions("Latin1_General_100_BIN2", "Ordinal", true, "Latin1_General_100_BIN2");
    }

    [Theory]
    [InlineData("Ordinal")]
    [InlineData("OrdinalIgnoreCase")]
    [InlineData("InvariantCulture")]
    [InlineData("InvariantCultureIgnoreCase")]
    [InlineData("ar")]
    [InlineData("az")]
    [InlineData("cs")]
    [InlineData("da")]
    [InlineData("de")]
    [InlineData("el")]
    [InlineData("en")]
    [InlineData("es")]
    [InlineData("fi")]
    [InlineData("fr")]
    [InlineData("he")]
    [InlineData("hu")]
    [InlineData("it")]
    [InlineData("ja")]
    [InlineData("ko")]
    [InlineData("nb")]
    [InlineData("nl")]
    [InlineData("nn")]
    [InlineData("pl")]
    [InlineData("pt")]
    [InlineData("ro")]
    [InlineData("ru")]
    [InlineData("sv")]
    [InlineData("tr")]
    [InlineData("uk")]
    [InlineData("zh-CN")]
    [InlineData("zh-TW")]
    public void SqlServer_Has_All_Mapped_Collations(string comparer)
    {
        if (SqlIntegrationConnections.ShouldSkip("SqlServer"))
            return;

        using var connection = SqlIntegrationConnections.CreateConnection("SqlServer");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sys.fn_helpcollations()";
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var reader = command.ExecuteReader())
            while (reader.Read())
                names.Add(reader.GetString(0));

        var isCulture = !comparer.Contains("Ordinal") && !comparer.Contains("Invariant");
        foreach (var options in isCulture ? new[] { "", ":IgnoreCase", ":IgnoreAccents", ":IgnoreCase:IgnoreAccents" } : [""])
        {
            Assert.True(SqlCollationSpec.TryParse(comparer + options, out var spec));
            var name = SqlCollationMapper.Map("SqlServer", spec, SqlCollationTarget.Database);
            Assert.Contains(name, names);
        }
    }
}
