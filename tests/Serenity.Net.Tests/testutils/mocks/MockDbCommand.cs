using System.Data.Common;

namespace Serenity.TestUtils;

public class MockDbCommand(IDbConnection connection = null) : DbCommand
{
    public override string CommandText { get; set; }
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override bool DesignTimeVisible { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    protected override DbConnection DbConnection { get => (DbConnection)connection; set => connection = value; }
    protected override DbParameterCollection DbParameterCollection { get; } = new MockDbParameterCollection();
    protected override DbTransaction DbTransaction { get; set; }

    public override void Cancel()
    {
    }

    protected override DbParameter CreateDbParameter()
    {
        return new MockDbParameter();
    }

    public MockDbCommand OnExecuteNonQuery(Func<int> func)
    {
        onExecuteNonQuery = func;
        return this;
    }

    protected Func<int> onExecuteNonQuery;

    public override int ExecuteNonQuery()
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

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        if (onExecuteReader != null)
            return onExecuteReader();

        throw new NotImplementedException();
    }

    public MockDbCommand OnExecuteScalar(Func<object> func)
    {
        onExecuteScalar = func;
        return this;
    }

    protected Func<object> onExecuteScalar;

    public override object ExecuteScalar()
    {
        if (onExecuteScalar != null)
            return onExecuteScalar();

        throw new NotImplementedException();
    }

    public override void Prepare()
    {
    }
}