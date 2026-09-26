using System.Collections;
using System.Data.Common;
using System.Threading;

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

    public OptionalValue<IRow> FindRow(FindRowArgs args)
    {
        var interceptArgs = new InterceptFindRowArgs(args.RowType, args.Id, args.Query, args.ByIdOrSingle);
        FindRowCalls.Add(interceptArgs);
        return interceptFindRow?.Invoke(interceptArgs) ?? default;
    }

    protected Func<InterceptListRowsArgs, OptionalValue<IList>> interceptListRows;

    public MockDbConnection InterceptListRows(Func<InterceptListRowsArgs, OptionalValue<IList>> callback)
    {
        this.interceptListRows = callback;
        return this;
    }

    public readonly List<InterceptListRowsArgs> ListRowsCalls = [];

    public OptionalValue<IList> ListRows(ListRowsArgs args)
    {
        var interceptArgs = new InterceptListRowsArgs(args.RowType, args.Query, args.CountOnly);
        ListRowsCalls.Add(interceptArgs);
        return interceptListRows?.Invoke(interceptArgs) ?? default;
    }

    protected Func<InterceptManipulateRowArgs, OptionalValue<long?>> interceptManipulateRow;

    public MockDbConnection InterceptManipulateRow(Func<InterceptManipulateRowArgs, OptionalValue<long?>> callback)
    {
        interceptManipulateRow = callback;
        return this;
    }

    public readonly List<InterceptManipulateRowArgs> ManipulateRowCalls = [];

    public OptionalValue<long?> ManipulateRow(Type rowType, OptionalValue<object> id, IRow row, ExpectedRows expectedRows, bool getNewId)
    {
        var args = new InterceptManipulateRowArgs(rowType, id, row, expectedRows, getNewId);
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

    public OptionalValue<long?> ExecuteNonQuery(string commandText, IDictionary<string, object?>? parameters, ExpectedRows expectedRows, IQueryWithParams query, bool getNewId)
    {
        var args = new InterceptExecuteNonQueryArgs(commandText, parameters, expectedRows, query, getNewId);
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

    public OptionalValue<IDataReader> ExecuteReader(string commandText, IDictionary<string, object?>? parameters, SqlQuery query)
    {
        var args = new InterceptExecuteReaderArgs(commandText, parameters, query);
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

    public OptionalValue<object> ExecuteScalar(string commandText, IDictionary<string, object?>? parameters, SqlQuery query)
    {
        var args = new InterceptExecuteScalarArgs(commandText, parameters, query);
        ExecuteScalarCalls.Add(args);
        return interceptExecuteScalar?.Invoke(args) ?? default;
    }

    public async Task<OptionalValue<IRow>> FindRowAsync(FindRowArgs args, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var interceptArgs = new InterceptFindRowArgs(args.RowType, args.Id, args.Query, args.ByIdOrSingle, IsAsync: true);
        FindRowCalls.Add(interceptArgs);
        return interceptFindRow?.Invoke(interceptArgs) ?? default;
    }

    public async Task<OptionalValue<IList>> ListRowsAsync(ListRowsArgs args, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var interceptArgs = new InterceptListRowsArgs(args.RowType, args.Query, args.CountOnly, IsAsync: true);
        ListRowsCalls.Add(interceptArgs);
        return interceptListRows?.Invoke(interceptArgs) ?? default;
    }

    public async Task<OptionalValue<long?>> ManipulateRowAsync(Type rowType, OptionalValue<object> id, IRow row, ExpectedRows expectedRows, bool getNewId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var args = new InterceptManipulateRowArgs(rowType, id, row, expectedRows, getNewId, IsAsync: true);
        ManipulateRowCalls.Add(args);
        return interceptManipulateRow?.Invoke(args) ?? default;
    }

    public async Task<OptionalValue<long?>> ExecuteNonQueryAsync(string commandText, IDictionary<string, object?>? parameters, ExpectedRows expectedRows, IQueryWithParams query, bool getNewId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var args = new InterceptExecuteNonQueryArgs(commandText, parameters, expectedRows, query, getNewId, IsAsync: true);
        ExecuteNonQueryCalls.Add(args);
        return interceptExecuteNonQuery?.Invoke(args) ?? default;
    }

    public async Task<OptionalValue<IDataReader>> ExecuteReaderAsync(string commandText, IDictionary<string, object?>? parameters, SqlQuery query, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var args = new InterceptExecuteReaderArgs(commandText, parameters, query, IsAsync: true);
        ExecuteReaderCalls.Add(args);
        return interceptExecuteReader?.Invoke(args) ?? default;
    }

    public async Task<OptionalValue<object>> ExecuteScalarAsync(string commandText, IDictionary<string, object?>? parameters, SqlQuery query, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        var args = new InterceptExecuteScalarArgs(commandText, parameters, query, IsAsync: true);
        ExecuteScalarCalls.Add(args);
        return interceptExecuteScalar?.Invoke(args) ?? default;
    }

    
    public int InterceptedCallCount =>
        FindRowCalls.Count + ListRowsCalls.Count + ManipulateRowCalls.Count +
        ExecuteNonQueryCalls.Count + ExecuteReaderCalls.Count + ExecuteScalarCalls.Count;

    public int AllCallCount => DbCommandCallCount + InterceptedCallCount;

    public ISqlDialect Dialect { get; set; }
}

public record InterceptFindRowArgs(Type Type, OptionalValue<object?> Id, SqlQuery Query, bool ByIdOrSingle, bool IsAsync = false);
public record InterceptListRowsArgs(Type Type, SqlQuery Query, bool CountOnly, bool IsAsync = false);
public record InterceptManipulateRowArgs(Type Type, OptionalValue<object> Id, IRow Row, ExpectedRows ExpectedRows, bool GetNewId, bool IsAsync = false);
public record InterceptExecuteNonQueryArgs(string CommandText, IDictionary<string, object?>? Parameters, ExpectedRows ExpectedRows, IQueryWithParams Query, bool GetNewId, bool IsAsync = false);
public record InterceptExecuteReaderArgs(string CommandText, IDictionary<string, object?>? Parameters, SqlQuery Query, bool IsAsync = false);
public record InterceptExecuteScalarArgs(string CommandText, IDictionary<string, object?>? Parameters, SqlQuery Query, bool IsAsync = false);
