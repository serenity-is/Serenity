namespace Serenity.Data;

public class SqlHelperNewCommandTests
{
    [Fact]
    public void NewCommand_NullConnection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SqlHelper.NewCommand(null!, "SELECT 1"));
    }

    [Fact]
    public void NewCommand_SetsCommandText()
    {
        using var connection = new MockDbConnection();

        using var command = SqlHelper.NewCommand(connection, "SELECT 1");

        Assert.Equal("SELECT 1", command.CommandText);
        Assert.Same(connection, command.Connection);
    }

    [Fact]
    public void NewCommand_TranslatesCommandText_ForDialect()
    {
        using var connection = new MockDbConnection { Dialect = PostgresDialect.Instance };

        using var command = SqlHelper.NewCommand(connection, "SELECT * FROM [Table] WHERE [X] = 1");

        Assert.Equal("SELECT * FROM \"Table\" WHERE \"X\" = 1", command.CommandText);
    }

    [Fact]
    public void NewCommand_WithNullParam_CreatesCommandWithoutParameters()
    {
        using var connection = new MockDbConnection();

        using var command = SqlHelper.NewCommand(connection, "SELECT 1", null);

        Assert.Empty(command.Parameters);
    }

    [Fact]
    public void NewCommand_WithEmptyParam_CreatesCommandWithoutParameters()
    {
        using var connection = new MockDbConnection();

        using var command = SqlHelper.NewCommand(connection, "SELECT 1", new Dictionary<string, object?>());

        Assert.Empty(command.Parameters);
    }

    [Fact]
    public void NewCommand_WithParams_AddsParameters()
    {
        using var connection = new MockDbConnection();

        using var command = SqlHelper.NewCommand(connection, "SELECT 1",
            new Dictionary<string, object?> { ["@p1"] = 5, ["@p2"] = "test" });

        Assert.Equal(2, command.Parameters.Count);
        Assert.Equal("@p1", ((IDbDataParameter)command.Parameters[0]!).ParameterName);
        Assert.Equal(5, ((IDbDataParameter)command.Parameters[0]!).Value);
        Assert.Equal("@p2", ((IDbDataParameter)command.Parameters[1]!).ParameterName);
        Assert.Equal("test", ((IDbDataParameter)command.Parameters[1]!).Value);
    }

    [Fact]
    public void NewCommand_WithParams_UsesConnectionDialect()
    {
        // Oracle is the only built-in dialect with a non-@ parameter prefix
        using var connection = new MockDbConnection { Dialect = OracleDialect.Instance };

        using var command = SqlHelper.NewCommand(connection, "SELECT 1",
            new Dictionary<string, object?> { ["@p1"] = 5 });

        Assert.Equal(":p1", ((IDbDataParameter)command.Parameters[0]!).ParameterName);
    }
}
