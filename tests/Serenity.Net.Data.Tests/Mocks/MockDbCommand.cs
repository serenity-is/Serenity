using System.Data.Common;
using System.Threading.Tasks;

namespace Serenity.Tests;

public class MockDbCommand : DbCommand
{
    public override string CommandText { get; set; }
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    protected override DbConnection DbConnection { get; set; }
    protected override DbParameterCollection DbParameterCollection { get; } = new MockDbParameterCollection();
    protected override DbTransaction DbTransaction { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    public override bool DesignTimeVisible { get; set; }

    public override void Cancel()
    {
    }

    protected override DbParameter CreateDbParameter()
    {
        return new MockDbParameter();
    }

    public override ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
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

        return new MockDataReader();
    }

    public override int ExecuteNonQuery()
    {
        throw new NotImplementedException();
    }

    public override object ExecuteScalar()
    {
        throw new NotImplementedException();
    }

    public override void Prepare()
    {
    }
}