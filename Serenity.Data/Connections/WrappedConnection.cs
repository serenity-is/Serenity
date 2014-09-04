using System.Data;

namespace Serenity.Data
{
    public class WrappedConnection : IWrappedConnection
    {
        private IDbConnection connection;
        private IDbTransaction transaction;

        public WrappedConnection(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IDbConnection Connection
        {
            get { return connection; }
        }

        public IDbTransaction Transaction
        {
            get { return transaction; }
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            transaction = connection.BeginTransaction(il);
            return new WrappedTransaction(this, transaction);
        }

        public IDbTransaction BeginTransaction()
        {
            transaction = connection.BeginTransaction();
            return new WrappedTransaction(this, transaction);
        }

        internal void Release(IDbTransaction transaction)
        {
            if (this.transaction == transaction)
            {
                this.transaction = null;
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            connection.Close();
        }

        public string ConnectionString
        {
            get
            {
                return connection.ConnectionString;
            }
            set
            {
                connection.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            var command = connection.CreateCommand();
            try
            {
                command.Transaction = this.transaction;
            }
            catch
            {
                command.Dispose();
                throw;
            }

            return command;
        }

        public string Database
        {
            get { return connection.Database; }
        }

        public void Open()
        {
            connection.Open();
        }

        public ConnectionState State
        {
            get { return connection.State; }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}