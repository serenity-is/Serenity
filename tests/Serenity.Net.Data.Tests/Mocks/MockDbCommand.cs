using System.Data.Common;

namespace Serenity.Tests;

public class MockDbCommand : IDbCommand
{
    public string CommandText { get; set; }
    public int CommandTimeout { get; set; }
    public CommandType CommandType { get; set; }
    public IDbConnection Connection { get; set; }
    public IDataParameterCollection Parameters { get; init; } = new MockDbParameterCollection();
    public IDbTransaction Transaction { get; set; }
    public UpdateRowSource UpdatedRowSource { get; set; }

    public MockDbCommand(IDbConnection connection = null)
    {
        Connection = connection;
    }

    public void Cancel()
    {
    }

    public IDbDataParameter CreateParameter()
    {
        return new MockDbParameter();
    }

    public void Dispose()
    {
    }

    public MockDbCommand OnExecuteNonQuery(Func<MockDbCommand, int> func)
    {
        onExecuteNonQuery = func;
        return this;
    }

    protected Func<MockDbCommand, int> onExecuteNonQuery;

    public int ExecuteNonQuery()
    {
        if (onExecuteNonQuery != null)
            return onExecuteNonQuery(this);

        return 1;
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

        return new MockDbDataReader();
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