using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Serenity.Data;

public class SqlHelperMiscTests
{
    [Fact]
    public void FixCommandText_TranslatesBracketsForDialect()
    {
#pragma warning disable CS0618 // obsolete wrapper over SqlConversions.Translate
        var result = SqlHelper.FixCommandText("SELECT * FROM [Table]", PostgresDialect.Instance);
#pragma warning restore CS0618

        Assert.Contains("\"Table\"", result);
    }

    [Fact]
    public void AddParamWithValue_CustomQueryParameter_AddsViaDapper()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new CustomQueryParameter(),
            SqlServer2012Dialect.Instance);

        Assert.Equal("@p1", param.ParameterName);
        Assert.Equal(5, param.Value);
        Assert.Same(param, command.Parameters[0]);
    }

    [Fact]
    public void NewCommand_WhenParamProcessingThrows_DisposesCommandAndRethrows()
    {
        using var connection = new MockDbConnection { Dialect = new ThrowingBoolWorkaroundDialect() };

        Assert.Throws<InvalidOperationException>(() =>
            SqlHelper.NewCommand(connection, "SELECT 1",
                new Dictionary<string, object?> { ["@p1"] = true }));
    }

    [Fact]
    public void ExecuteNonQuery_WithConnectionPoolException_ClosesReopensAndRetries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return 7;
            });

        var result = SqlHelper.ExecuteNonQuery(connection, "DELETE FROM T");

        Assert.Equal(7, result);
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public void ExecuteNonQuery_WithOpenedOnceConnection_DoesNotRetryPoolException()
    {
        using var connection = new OpenedOnceConnection();
        connection.Open();
        connection.OnDbCommandExecuteNonQuery(_ => throw new System.Data.SqlException(10054));

        var ex = Assert.Throws<System.Data.SqlException>(() =>
            SqlHelper.ExecuteNonQuery(connection, "DELETE FROM T"));

        Assert.Equal("DELETE FROM T", ex.Data["sql_command_text"]);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_WithConnectionPoolException_ClosesReopensAndRetries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return 3;
            });

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "DELETE FROM T",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(3, result);
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public void ExecuteReader_WithConnectionPoolException_Retries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return new MockDbDataReader(new { X = 1 });
            });

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT X", null);

        Assert.True(reader.Read());
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public void ExecuteScalar_WithConnectionPoolException_Retries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return 42;
            });

        var result = SqlHelper.ExecuteScalar(connection, "SELECT X");

        Assert.Equal(42, result);
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_NonDbConnection_WithPoolException_Retries()
    {
        var calls = 0;
        using var inner = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return 5;
            });
        using var connection = new PlainDbConnection(inner);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "DELETE FROM T",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(5, result);
        Assert.Equal(2, calls);
        Assert.Equal(2, inner.OpenCalls);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_NonDbCommand_ExecutesSynchronously()
    {
        using var inner = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 5);
        using var connection = new PlainDbConnection(inner);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "DELETE FROM T",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(5, result);
        Assert.Equal(1, inner.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteReaderAsync_NonDbCommand_ExecutesSynchronously()
    {
        using var inner = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { X = (long?)1 }));
        using var connection = new PlainDbConnection(inner);

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT X",
            null, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        Assert.Equal(1, inner.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExecuteScalarAsync_NonDbCommand_ExecutesSynchronously()
    {
        using var inner = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => 42);
        using var connection = new PlainDbConnection(inner);

        var result = await SqlHelper.ExecuteScalarAsync(connection, "SELECT X",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        Assert.Equal(1, inner.DbCommandExecuteScalarCallCount);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_WithDebugLogger_LogsCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);
        var logger = new MockLogger();

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "DELETE FROM T",
            logger: logger, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Null(logger.LastException);
    }

    [Fact]
    public void ExecuteReader_WithDebugLogger_LogsCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { X = 1 }));
        var logger = new MockLogger();

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT X", null, logger: logger);

        Assert.True(reader.Read());
        Assert.Null(logger.LastException);
    }

    [Fact]
    public async Task ExecuteReaderAsync_WithDebugLogger_LogsCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { X = 1 }));
        var logger = new MockLogger();

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT X",
            null, logger: logger, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        Assert.Null(logger.LastException);
    }

    [Fact]
    public void ExecuteScalar_WithDebugLogger_LogsCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => 42);
        var logger = new MockLogger();

        var result = SqlHelper.ExecuteScalar(connection, "SELECT X", logger: logger);

        Assert.Equal(42, result);
        Assert.Null(logger.LastException);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WithDebugLogger_LogsCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => 42);
        var logger = new MockLogger();

        var result = await SqlHelper.ExecuteScalarAsync(connection, "SELECT X",
            logger: logger, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        Assert.Null(logger.LastException);
    }

    [Fact]
    public void ExecuteUpsert_UnknownDialect_WithParams_CopiesParamsToFallbackUpdate()
    {
        var calls = 0;
        using var connection = new MockDbConnection { Dialect = new UnknownUpsertDialect() }
            .OnDbCommandExecuteNonQuery(_ => ++calls); // update affects 1 row, insert skipped

        var query = new SqlInsert("Table").SetTo("Id", "@id").SetTo("X", "@x");
        query.AddParam("@id", 1);
        query.AddParam("@x", "test");

        var result = query.ExecuteUpsert(connection, ["Id"]);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteUpsertAsync_UnknownDialect_WithUpdateMiss_ExecutesInsert()
    {
        var calls = 0;
        using var connection = new MockDbConnection { Dialect = new UnknownUpsertDialect() }
            .OnDbCommandExecuteNonQuery(_ => ++calls == 1 ? 0 : 1); // update misses, insert runs

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");

        var result = await query.ExecuteUpsertAsync(connection, ["Id"],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(2, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteReaderAsync_WithConnectionPoolException_Retries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return new MockDbDataReader(new { X = 1 });
            });

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT X",
            null, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WithConnectionPoolException_Retries()
    {
        var calls = 0;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ =>
            {
                if (++calls == 1)
                    throw new System.Data.SqlException(10054);
                return 42;
            });

        var result = await SqlHelper.ExecuteScalarAsync(connection, "SELECT X",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        Assert.Equal(2, calls);
        Assert.Equal(2, connection.OpenCalls);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WithSqlQuery_Executes()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => 42);

        var query = new SqlQuery().From("Table").Select("X");
        var result = await SqlHelper.ExecuteScalarAsync(connection, query,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
    }

    private class CustomQueryParameter : Dapper.SqlMapper.ICustomQueryParameter
    {
        public void AddParameter(IDbCommand command, string name)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = 5;
            command.Parameters.Add(param);
        }
    }

    private class ThrowingBoolWorkaroundDialect : SqlServer2012Dialect
    {
        public override bool NeedsBoolWorkaround => throw new InvalidOperationException("boom");
    }

    private class OpenedOnceConnection : MockDbConnection, IHasOpenedOnce
    {
        public bool OpenedOnce => true;
    }

    private class UnknownUpsertDialect : SqlServer2012Dialect
    {
        public override string ServerType => "UnknownServer";
    }

    /// <summary>
    /// An IDbCommand that is not a DbCommand, to exercise the synchronous
    /// fallback paths of the async execution helpers.
    /// </summary>
    private class PlainCommand(IDbCommand inner) : IDbCommand
    {
        public string CommandText { get => inner.CommandText; set => inner.CommandText = value; }
        public int CommandTimeout { get => inner.CommandTimeout; set => inner.CommandTimeout = value; }
        public CommandType CommandType { get => inner.CommandType; set => inner.CommandType = value; }
        public IDbConnection? Connection { get => ((IDbCommand)inner).Connection; set => ((IDbCommand)inner).Connection = value!; }
        public IDataParameterCollection Parameters => inner.Parameters;
        public IDbTransaction? Transaction { get => ((IDbCommand)inner).Transaction; set => ((IDbCommand)inner).Transaction = value!; }
        public UpdateRowSource UpdatedRowSource { get => inner.UpdatedRowSource; set => inner.UpdatedRowSource = value; }

        public void Cancel() => inner.Cancel();
        public IDbDataParameter CreateParameter() => inner.CreateParameter();
        public void Dispose() => inner.Dispose();
        public int ExecuteNonQuery() => inner.ExecuteNonQuery();
        public IDataReader ExecuteReader() => inner.ExecuteReader();
        public IDataReader ExecuteReader(CommandBehavior behavior) => inner.ExecuteReader(behavior);
        public object? ExecuteScalar() => inner.ExecuteScalar();
        public void Prepare() => inner.Prepare();
    }

    /// <summary>
    /// An IDbConnection that is not a DbConnection, to exercise the
    /// non-DbConnection branches of the async helpers.
    /// </summary>
    private class PlainDbConnection(MockDbConnection inner) : IDbConnection
    {
        public string ConnectionString { get => inner.ConnectionString; set => inner.ConnectionString = value; }
        public int ConnectionTimeout => inner.ConnectionTimeout;
        public string Database => inner.Database;
        public ConnectionState State => inner.State;

        public void ChangeDatabase(string databaseName) => inner.ChangeDatabase(databaseName);
        public IDbTransaction BeginTransaction() => inner.BeginTransaction();
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel) => inner.BeginTransaction(isolationLevel);
        public IDbCommand CreateCommand() => new PlainCommand(inner.CreateCommand());
        public void Open() => inner.Open();
        public void Close() => inner.Close();
        public void Dispose() => inner.Dispose();
    }
}
