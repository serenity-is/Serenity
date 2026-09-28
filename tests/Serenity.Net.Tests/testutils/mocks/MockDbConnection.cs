using System.Collections;
using System.Data.Common;
using System.Threading;
using Serenity.Services;

namespace Serenity.TestUtils;

public class MockDbConnection : DbConnection, IRowOperationInterceptor, ISqlOperationInterceptor, IHasDialect
{
    private ConnectionState state = ConnectionState.Closed;
    public override string ConnectionString { get; set; }
    private string database = string.Empty;
    public override string Database => database;
    public override int ConnectionTimeout => 0;
    public override string DataSource => throw new NotImplementedException();
    public override string ServerVersion => throw new NotImplementedException();
    public override ConnectionState State => state;

    public MockDbConnection()
    {
        state = ConnectionState.Closed;
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return new MockDbTransaction(this);
    }

    public override void ChangeDatabase(string databaseName)
    {
        database = databaseName;
    }

    public override void Close()
    {
        state = ConnectionState.Closed;
    }

    protected override DbCommand CreateDbCommand()
    {
        var command = new MockDbCommand(this, CreatedParameterDbType);

        if (onDbCommandExecuteReader != null)
            command.OnExecuteReader(() => 
            {
                DbCommandExecuteReaderCallCount++;
                return onDbCommandExecuteReader(command);
            });

        if (onDbCommandExecuteNonQuery != null)
        {
            command.OnExecuteNonQuery(() => 
            {
                DbCommandExecuteNonQueryCallCount++;
                return onDbCommandExecuteNonQuery(command);
            });
        }

        if (onDbCommandExecuteScalar != null)
        {
            command.OnExecuteScalar(() =>
            {
                DbCommandExecuteScalarCallCount++;
                return onDbCommandExecuteScalar(command);
            });
        }

        return command;
    }

    public int OpenCalls { get; protected set; }
    public DbType? CreatedParameterDbType { get; set; }
    public int DbCommandExecuteReaderCallCount { get; protected set; } = 0;
    public int DbCommandExecuteNonQueryCallCount { get; protected set; } = 0;
    public int DbCommandExecuteScalarCallCount { get; protected set; } = 0;
    public int DbCommandCallCount => DbCommandExecuteReaderCallCount + DbCommandExecuteNonQueryCallCount + DbCommandExecuteScalarCallCount;

    protected Func<MockDbCommand, DbDataReader> onDbCommandExecuteReader;
    protected Func<MockDbCommand, int> onDbCommandExecuteNonQuery;
    protected Func<MockDbCommand, object> onDbCommandExecuteScalar;

    public MockDbConnection OnDbCommandExecuteReader(Func<MockDbCommand, DbDataReader> func)
    {
        onDbCommandExecuteReader = func;
        return this;
    }

    public MockDbConnection OnDbCommandExecuteNonQuery(Func<MockDbCommand, int> func)
    {
        onDbCommandExecuteNonQuery = func;
        return this;
    }

    public MockDbConnection OnDbCommandExecuteScalar(Func<MockDbCommand, object> func)
    {
        onDbCommandExecuteScalar = func;
        return this;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Close();
        base.Dispose(disposing);
    }

    public override void Open()
    {
        if (state != ConnectionState.Closed &&
            state != ConnectionState.Broken)
            throw new InvalidOperationException("The connection is already open!");
        state = ConnectionState.Open;
        OpenCalls++;
    }


    protected Func<InterceptFindRowArgs, OptionalValue<IRow>> interceptFindRow;

    public MockDbConnection InterceptFindRow(Func<InterceptFindRowArgs, OptionalValue<IRow>> callback)
    {
        this.interceptFindRow = callback;
        return this;
    }

    public readonly List<InterceptFindRowArgs> FindRowCalls = [];

    public OptionalValue<IRow> FindRow(InterceptFindRowArgs args)
    {
        FindRowCalls.Add(args);
        return interceptFindRow?.Invoke(args) ?? default;
    }

    protected Func<InterceptListRowsArgs, OptionalValue<IList>> interceptListRows;

    public MockDbConnection InterceptListRows(Func<InterceptListRowsArgs, OptionalValue<IList>> callback)
    {
        this.interceptListRows = callback;
        return this;
    }

    public readonly List<InterceptListRowsArgs> ListRowsCalls = [];

    public OptionalValue<IList> ListRows(InterceptListRowsArgs args)
    {
        ListRowsCalls.Add(args);
        return interceptListRows?.Invoke(args) ?? default;
    }

    protected Func<InterceptManipulateRowArgs, OptionalValue<long?>> interceptManipulateRow;

    public MockDbConnection InterceptManipulateRow(Func<InterceptManipulateRowArgs, OptionalValue<long?>> callback)
    {
        interceptManipulateRow = callback;
        return this;
    }

    public readonly List<InterceptManipulateRowArgs> ManipulateRowCalls = [];

    public OptionalValue<long?> ManipulateRow(InterceptManipulateRowArgs args)
    {
        ManipulateRowCalls.Add(args);
        return interceptManipulateRow?.Invoke(args) ?? default;
    }

    protected Func<InterceptExecuteNonQueryArgs, OptionalValue<long?>> interceptExecuteNonQuery;

    public MockDbConnection InterceptExecuteNonQuery(Func<InterceptExecuteNonQueryArgs, OptionalValue<long?>> callback)
    {
        interceptExecuteNonQuery = callback;
        return this;
    }

    public readonly List<InterceptExecuteNonQueryArgs> ExecuteNonQueryCalls = [];

    public OptionalValue<long?> ExecuteNonQuery(InterceptExecuteNonQueryArgs args)
    {
        ExecuteNonQueryCalls.Add(args);
        return interceptExecuteNonQuery?.Invoke(args) ?? default;
    }


    protected Func<InterceptExecuteReaderArgs, OptionalValue<IDataReader>> interceptExecuteReader;

    public MockDbConnection InterceptExecuteReader(Func<InterceptExecuteReaderArgs, OptionalValue<IDataReader>> callback)
    {
        interceptExecuteReader = callback;
        return this;
    }

    public readonly List<InterceptExecuteReaderArgs> ExecuteReaderCalls = [];

    public OptionalValue<IDataReader> ExecuteReader(InterceptExecuteReaderArgs args)
    {
        ExecuteReaderCalls.Add(args);
        return interceptExecuteReader?.Invoke(args) ?? default;
    }

    protected Func<InterceptExecuteScalarArgs, OptionalValue<object>> interceptExecuteScalar;

    public MockDbConnection InterceptExecuteScalar(Func<InterceptExecuteScalarArgs, OptionalValue<object>> callback)
    {
        interceptExecuteScalar = callback;
        return this;
    }

    public readonly List<InterceptExecuteScalarArgs> ExecuteScalarCalls = [];

    public OptionalValue<object> ExecuteScalar(InterceptExecuteScalarArgs args)
    {
        ExecuteScalarCalls.Add(args);
        return interceptExecuteScalar?.Invoke(args) ?? default;
    }

    public async Task<OptionalValue<IRow>> FindRowAsync(InterceptFindRowArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var interceptArgs = args with { IsAsync = true };
        FindRowCalls.Add(interceptArgs);
        return interceptFindRow?.Invoke(interceptArgs) ?? default;
    }

    public async Task<OptionalValue<IList>> ListRowsAsync(InterceptListRowsArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var interceptArgs = args with { IsAsync = true };
        ListRowsCalls.Add(interceptArgs);
        return interceptListRows?.Invoke(interceptArgs) ?? default;
    }

    public async Task<OptionalValue<long?>> ManipulateRowAsync(InterceptManipulateRowArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var asyncArgs = args with { IsAsync = true };
        ManipulateRowCalls.Add(asyncArgs);
        return interceptManipulateRow?.Invoke(asyncArgs) ?? default;
    }

    public async Task<OptionalValue<long?>> ExecuteNonQueryAsync(InterceptExecuteNonQueryArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var asyncArgs = args with { IsAsync = true };
        ExecuteNonQueryCalls.Add(asyncArgs);
        return interceptExecuteNonQuery?.Invoke(asyncArgs) ?? default;
    }

    public async Task<OptionalValue<IDataReader>> ExecuteReaderAsync(InterceptExecuteReaderArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var asyncArgs = args with { IsAsync = true };
        ExecuteReaderCalls.Add(asyncArgs);
        return interceptExecuteReader?.Invoke(asyncArgs) ?? default;
    }

    public async Task<OptionalValue<object>> ExecuteScalarAsync(InterceptExecuteScalarArgs args)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var asyncArgs = args with { IsAsync = true };
        ExecuteScalarCalls.Add(asyncArgs);
        return interceptExecuteScalar?.Invoke(asyncArgs) ?? default;
    }

    
    public int InterceptedCallCount =>
        FindRowCalls.Count + ListRowsCalls.Count + ManipulateRowCalls.Count +
        ExecuteNonQueryCalls.Count + ExecuteReaderCalls.Count + ExecuteScalarCalls.Count;

    public int AllCallCount => DbCommandCallCount + InterceptedCallCount;

    public ISqlDialect Dialect { get; set; }
}

