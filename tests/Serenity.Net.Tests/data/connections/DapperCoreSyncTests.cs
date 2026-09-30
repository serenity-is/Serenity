namespace Serenity.Data;

public class DapperCoreSyncTests
{
    private class TestRow
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private static SqlQuery CreateQuery()
    {
        var query = new SqlQuery()
            .From("Table")
            .Select("Id")
            .Select("Name")
            .Where("Id = @Id");
        query.AddParam("@Id", 1);
        return query;
    }

    [Fact]
    public void Query_T_ISqlQuery_OracleDialect_TranslatesPrefix_AndBindsParameter()
    {
        // Translate() rewrites @Id to :Id in the SQL text while DynamicParameters
        // keeps the @Id key. Dapper matches them via its prefix-stripping Clean,
        // so no key normalization is needed here. This test locks that contract.

        string? capturedText = null;
        var capturedParams = new List<(string Name, object? Value)>();

        using var connection = new MockDbConnection { Dialect = OracleDialect.Instance }
            .OnDbCommandExecuteReader(command =>
            {
                capturedText = command.CommandText;
                foreach (IDbDataParameter p in command.Parameters)
                    capturedParams.Add((p.ParameterName, p.Value));
                return new MockDbDataReader(new { Id = 1 });
            });

        var list = connection.Query<TestRow>(CreateQuery()).ToList();

        Assert.Single(list);
        Assert.Contains(":Id", capturedText, StringComparison.Ordinal);
        var bound = Assert.Single(capturedParams);
        Assert.Equal("Id", bound.Name);
        Assert.Equal(1, bound.Value);
    }

    [Fact]
    public void Execute_WithAmbientTransaction_EnlistsCommand()
    {
        using var actual = new MockDbConnection();
        using var wrapped = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        using var tx = wrapped.BeginTransaction();

        IDbTransaction? captured = null;
        actual.OnDbCommandExecuteNonQuery(command =>
        {
            captured = command.Transaction;
            return 1;
        });

        wrapped.Execute("UPDATE [Table] SET [X] = 1");

        Assert.Same(wrapped.GetCurrentActualTransaction(), captured);
    }

    [Fact]
    public void Execute_Executes_AndReturnsRowsAffected()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(command => 5);

        var result = connection.Execute("UPDATE [Table] SET [X] = 1");

        Assert.Equal(5, result);
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void Query_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query("SELECT * FROM [Table]").ToList();

        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
    }

    [Fact]
    public void Query_ISqlQuery_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query(CreateQuery()).ToList();

        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
    }

    [Fact]
    public void Query_T_Returns_TypedObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query<TestRow>("SELECT * FROM [Table]").ToList();

        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
    }

    [Fact]
    public void Query_T_ISqlQuery_Returns_TypedObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query<TestRow>(CreateQuery()).ToList();

        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
    }
}
