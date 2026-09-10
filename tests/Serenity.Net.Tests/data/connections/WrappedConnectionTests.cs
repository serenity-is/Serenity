namespace Serenity.Data;

public class WrappedConnectionTests
{
    private static WrappedConnection GetWrapped()
    {
        return new WrappedConnection(new MockDbConnection(), SqlServer2012Dialect.Instance);
    }

    [Fact]
    public void Constructor_NullConnection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new WrappedConnection(null!, SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void Properties_DelegateToActualConnection()
    {
        var connection = new MockDbConnection { ConnectionString = "cs" };
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.ConnectionString = "test";
        Assert.Equal("test", wrapped.ConnectionString);
        Assert.Equal(0, wrapped.ConnectionTimeout);
        Assert.Equal(connection.Database, wrapped.Database);
        Assert.Same(connection, wrapped.ActualConnection);
        Assert.Null(wrapped.CurrentTransaction);
        Assert.Equal(SqlServer2012Dialect.Instance, wrapped.Dialect);
        Assert.Null(wrapped.Logger);
        Assert.False(wrapped.OpenedOnce);
        Assert.Equal(ConnectionState.Closed, wrapped.State);
    }

    [Fact]
    public void Open_SetsOpenedOnce()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.Open();
        Assert.True(wrapped.OpenedOnce);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void Close_DelegatesToActualConnection()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.Open();
        wrapped.Close();
        Assert.Equal(ConnectionState.Closed, wrapped.State);

        wrapped.ChangeDatabase("Db");
        Assert.Equal("Db", connection.Database);
    }

    [Fact]
    public void CommandTimeout_Setting_AppliesToCreatedCommands()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance };
        var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance)
        {
            CommandTimeout = 42
        };

        var command = ((IDbConnection)wrapped).CreateCommand();
        Assert.Equal(42, command.CommandTimeout);

        var dbCommand = wrapped.CreateCommand();
        Assert.Equal(42, dbCommand.CommandTimeout);

        wrapped.Dispose();
        connection.Dispose();
    }

    [Fact]
    public void CreateCommand_WithoutDialect_UsesActualConnectionCommand()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, null!);

        var command = ((IDbConnection)wrapped).CreateCommand();
        Assert.IsType<MockDbCommand>(command);
    }

    [Fact]
    public void DataSource_AndServerVersion_ForDbConnection_Delegate()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        Assert.Throws<NotImplementedException>(() => wrapped.DataSource);
        Assert.Throws<NotImplementedException>(() => wrapped.ServerVersion);
    }

    [Fact]
    public void BeginTransaction_WrapsAndTracksTransaction()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance };
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        var transaction = wrapped.BeginTransaction();
        Assert.IsType<WrappedTransaction>(transaction);
        Assert.Same(transaction, wrapped.CurrentTransaction);

        transaction.Rollback();
        Assert.Null(wrapped.CurrentTransaction);
    }

    [Fact]
    public void Dispose_DisposesActualConnection()
    {
        var connection = new MockDbConnection();
        var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.Dispose();
        Assert.True(connection.State == ConnectionState.Closed);
    }
}
