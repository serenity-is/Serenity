using System.Data.Common;

namespace Serenity.Tests;

public class MockDbConnection : IDbConnection
{
    public virtual string ConnectionString { get; set; }
    public virtual int ConnectionTimeout { get; set; }
    public virtual string Database { get; set; }
    public virtual ConnectionState State { get; set; }

    public MockDbConnection()
    {
        State = ConnectionState.Closed;
    }

    public virtual IDbTransaction BeginTransaction()
    {
        return new MockDbTransaction(this);
    }

    public virtual IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return new MockDbTransaction(this);
    }

    public virtual void ChangeDatabase(string databaseName)
    {
        throw new NotImplementedException();
    }

    public virtual void Close()
    {
        State = ConnectionState.Closed;
    }

    public virtual IDbCommand CreateCommand()
    {
        var command = new MockDbCommand(this);

        if (onExecuteReader != null)
            command.OnExecuteReader(() => onExecuteReader(command));

        if (onExecuteNonQuery != null)
            command.OnExecuteNonQuery((command) => onExecuteNonQuery(command));

        if (onCreateCommand != null)
            return onCreateCommand(command);

        return command;
    }

    public MockDbConnection OnCreateCommand(Action<MockDbCommand> action)
    {
        onCreateCommand = command =>
        {
            action(command);
            return command;
        };

        return this;
    }

    protected Func<MockDbCommand, IDbCommand> onCreateCommand;
    protected Func<IDbCommand, DbDataReader> onExecuteReader;
    protected Func<MockDbCommand, int> onExecuteNonQuery;

    public MockDbConnection OnExecuteReader(Func<IDbCommand, DbDataReader> func)
    {
        onExecuteReader = func;
        return this;
    }


    public MockDbConnection OnExecuteNonQuery(Func<MockDbCommand, int> func)
    {
        onExecuteNonQuery = func;
        return this;
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public virtual void Dispose()
    {
        Close();
    }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    public virtual void Open()
    {
        if (State != ConnectionState.Closed &&
            State != ConnectionState.Broken)
            throw new InvalidOperationException("The connection is already open!");
    }
}