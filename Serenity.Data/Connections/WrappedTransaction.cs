using System.Data;

namespace Serenity.Data
{
    public class WrappedTransaction : IDbTransaction
    {
        private WrappedConnection connection;
        private IDbTransaction transaction;

        /// <summary>
        ///   Creates a new WrappedTransaction instance.</summary>
        /// <param name="transaction">
        ///   The actual transaction, this wrapped transaction is created for.</param>
        internal WrappedTransaction(WrappedConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        /// <summary>
        ///   Returns the connection associated with this transaction.</summary>
        public IDbConnection Connection
        {
            get { return connection ?? transaction.Connection; }
        }

        /// <summary>
        ///   Returns the actual transaction.</summary>
        public IDbTransaction Transaction
        {
            get { return transaction; }
        }

        /// <summary>
        ///   Returns the transaction isolation level</summary>
        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        /// <summary>
        ///   Commits actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Commit()
        {
            transaction.Commit();
            DetachConnection();
        }

        /// <summary>
        ///   Rollbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Rollback()
        {
            transaction.Rollback();
            DetachConnection();
        }

        /// <summary>
        ///   Rolbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Dispose()
        {
            transaction.Dispose();
            DetachConnection();
        }

        private void DetachConnection()
        {
            if (this.connection != null)
            {
                this.connection.Release(this.transaction);
                this.connection = null;
            }
        }
    }
}

