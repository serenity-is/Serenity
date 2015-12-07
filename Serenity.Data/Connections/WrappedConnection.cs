using System.Data;

namespace Serenity.Data
{
    public class WrappedConnection : IDbConnection
    {
        private IDbConnection actualConnection;
        private WrappedTransaction currentTransaction;
        private ISqlDialect dialect;

        public WrappedConnection(IDbConnection actualConnection, ISqlDialect dialect)
        {
            this.actualConnection = actualConnection;
            this.dialect = dialect;
        }

        public IDbConnection ActualConnection
        {
            get { return actualConnection; }
        }
        public ISqlDialect Dialect
        {
            get { return dialect; }
            set { dialect = value; }
        }

        public WrappedTransaction CurrentTransaction
        {
            get { return currentTransaction; }
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            var actualTransaction = actualConnection.BeginTransaction(il);
            currentTransaction = new WrappedTransaction(this, actualTransaction);
            return currentTransaction;
        }

        public IDbTransaction BeginTransaction()
        {
            var actualTransaction = actualConnection.BeginTransaction();
            currentTransaction = new WrappedTransaction(this, actualTransaction);
            return currentTransaction;
        }

        internal void Release(WrappedTransaction transaction)
        {
            if (this.currentTransaction == transaction)
            {
                this.currentTransaction = null;
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            actualConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            actualConnection.Close();
        }

        public string ConnectionString
        {
            get
            {
                return actualConnection.ConnectionString;
            }
            set
            {
                actualConnection.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return actualConnection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            var command = actualConnection.CreateCommand();
            try
            {
                command.Transaction = this.currentTransaction != null ? this.currentTransaction.ActualTransaction : null;
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
            get { return actualConnection.Database; }
        }

        public void Open()
        {
            actualConnection.Open();
        }

        public ConnectionState State
        {
            get { return actualConnection.State; }
        }

        public void Dispose()
        {
            actualConnection.Dispose();
        }
    }
}