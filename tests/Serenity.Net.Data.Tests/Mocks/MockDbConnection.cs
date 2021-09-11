using System;
using System.Data;
using System.Data.Common;

namespace Serenity.Tests
{
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
            throw new NotImplementedException();
        }

        public virtual IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
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
            var command = new MockDbCommand();
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

        public MockDbConnection OnExecuteReader(Func<IDbCommand, DbDataReader> func)
        {
            if (onCreateCommand != null)
                throw new InvalidOperationException("Can't use OnCreateCommand with OnExecuteReader!");

            return OnCreateCommand(command => command.OnExecuteReader(() => func(command)));
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
}