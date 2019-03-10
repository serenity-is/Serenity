using System.Data;

namespace Serenity.Data
{
    public class WrappedConnection : IDbConnection
    {
        private IDbConnection actualConnection;
        private bool openedOnce;
        private WrappedTransaction currentTransaction;
        private ISqlDialect dialect;
        private int? customCommandTimeout;

        public WrappedConnection(IDbConnection actualConnection, ISqlDialect dialect)
        {
            this.actualConnection = actualConnection;
            this.dialect = dialect;
        }

        public bool OpenedOnce
        {
            get { return openedOnce; }
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
                var transaction = this.currentTransaction != null ? this.currentTransaction.ActualTransaction : null;
                if (transaction != null && transaction.Connection == null)
                    throw new System.Exception("Active transaction for connection is in invalid state! " + 
                        "Connection was probably closed unexpectedly!");

                command.Transaction = transaction;

                if (transaction != null && command.Transaction == null)
                    throw new System.Exception("Can't set transaction for command! " +
                        "Connection was probably closed unexpectedly!");
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
            openedOnce = true;
        }

        public ConnectionState State
        {
            get { return actualConnection.State; }
        }

        public void Dispose()
        {
            actualConnection.Dispose();
        }

        public int? CustomCommandTimeout
        {
            get
            {
                return customCommandTimeout;
            }
            set
            {
                this.customCommandTimeout = value;
            }
        }
    }
}