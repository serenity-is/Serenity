using System.Data;

namespace Serenity.Data
{
    /// <summary>
    ///   Helper class returned from DataHelper.BeginTransaction method.</summary>
    public class AutoTransaction : IDbTransaction
    {
        private IDbTransaction _transaction;

        /// <summary>
        ///   Creates a new AutoTransaction instance.</summary>
        /// <param name="transaction">
        ///   The actual transaction, this auto transaction is created for.</param>
        internal AutoTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        /// <summary>
        ///   Returns the connection associated with this transaction.</summary>
        public IDbConnection Connection
        {
            get { return _transaction.Connection; }
        }

        /// <summary>
        ///   Returns the actual transaction.</summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }

        /// <summary>
        ///   Returns the transaction isolation level</summary>
        public IsolationLevel IsolationLevel
        {
            get { return _transaction.IsolationLevel; }
        }

        /// <summary>
        ///   Commits actual transaction and sets auto transaction for related connection to null.</summary>
        public void Commit()
        {
            var connection = _transaction.Connection;
            _transaction.Commit();
            SqlTransactions.SetDefaultTransaction(connection, null);
        }

        /// <summary>
        ///   Rollbacks actual transaction and sets auto transaction for related connection to null.</summary>
        public void Rollback()
        {
            var connection = _transaction.Connection;
            _transaction.Rollback();
            SqlTransactions.SetDefaultTransaction(connection, null);
        }

        /// <summary>
        ///   Rolbacks actual transaction and sets auto transaction for related connection to null.</summary>
        public void Dispose()
        {
            var connection = _transaction.Connection;
            _transaction.Dispose();
            SqlTransactions.SetDefaultTransaction(connection, null);
        }
    }
}
