using System.Data;

namespace Serenity.Data
{
    public class WrappedTransaction : IDbTransaction
    {
        private WrappedConnection wrappedConnection;
        private IDbTransaction actualTransaction;

        /// <summary>
        ///   Creates a new WrappedTransaction instance.</summary>
        /// <param name="transaction">
        ///   The actual transaction, this wrapped transaction is created for.</param>
        internal WrappedTransaction(WrappedConnection wrappedConnection, IDbTransaction actualTransaction)
        {
            this.wrappedConnection = wrappedConnection;
            this.actualTransaction = actualTransaction;
        }

        /// <summary>
        ///   Returns the connection associated with this transaction.</summary>
        public IDbConnection Connection
        {
            get { return wrappedConnection; }
        }

        /// <summary>
        ///   Returns the actual transaction.</summary>
        public IDbTransaction ActualTransaction
        {
            get { return actualTransaction; }
        }

        /// <summary>
        ///   Returns the transaction isolation level</summary>
        public IsolationLevel IsolationLevel
        {
            get { return actualTransaction.IsolationLevel; }
        }

        /// <summary>
        ///   Commits actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Commit()
        {
            actualTransaction.Commit();
            DetachConnection();
        }

        /// <summary>
        ///   Rollbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Rollback()
        {
            actualTransaction.Rollback();
            DetachConnection();
        }

        /// <summary>
        ///   Rolbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
        public void Dispose()
        {
            actualTransaction.Dispose();
            DetachConnection();
        }

        private void DetachConnection()
        {
            if (this.wrappedConnection != null)
            {
                this.wrappedConnection.Release(this);
                this.wrappedConnection = null;
            }
        }
    }
}

