using System.Data.Common;

namespace Serenity.TestUtils;

public class MockDbCommand(IDbConnection connection = null) : IDbCommand
{
    public string CommandText { get; set; }
    public int CommandTimeout { get; set; }
    public CommandType CommandType { get; set; }
    public IDbConnection Connection { get; set; } = connection;
    public IDataParameterCollection Parameters { get; init; } = new MockDbParameterCollection();
    public IDbTransaction Transaction { get; set; }
    public UpdateRowSource UpdatedRowSource { get; set; }

    public void Cancel()
    {
    }

    public IDbDataParameter CreateParameter()
    {
        return new MockDbParameter();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public MockDbCommand OnExecuteNonQuery(Func<int> func)
    {
        onExecuteNonQuery = func;
        return this;
    }

    protected Func<int> onExecuteNonQuery;

    public int ExecuteNonQuery()
    {
        if (onExecuteNonQuery != null)
            return onExecuteNonQuery();

        throw new NotImplementedException();
    }

    public MockDbCommand OnExecuteReader(Func<DbDataReader> func)
    {
        onExecuteReader = func;
        return this;
    }

    protected Func<DbDataReader> onExecuteReader;

    public IDataReader ExecuteReader()
    {
        if (onExecuteReader != null)
            return onExecuteReader();

        throw new NotImplementedException();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        return ExecuteReader();
    }

    public object ExecuteScalar()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
    }
}