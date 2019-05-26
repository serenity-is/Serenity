using System;
using System.Data;

namespace Serenity.Data
{
    /// <summary>
    /// Unit of work implementation.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Serenity.Data.IUnitOfWork" />
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private Action _commit;
        private Action _rollback;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        public UnitOfWork(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
            connection.EnsureOpen();
            _transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IDbConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Rollbacks the transaction if any.
        /// </summary>
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;

                if (_rollback != null)
                {
                    _rollback();
                    _rollback = null;
                }
            }
        }

        /// <summary>
        /// Commits this transaction.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        public void Commit()
        {
            if (_transaction == null)
                throw new ArgumentNullException("transaction");

            _transaction.Commit();
            _transaction = null;

            if (_commit != null)
            {
                _commit();
                _commit = null;
            }
        }

        /// <summary>
        /// Occurs when transaction is committed.
        /// </summary>
        public event Action OnCommit
        {
            add { _commit += value; }
            remove { _commit -= value; }
        }

        /// <summary>
        /// Occurs when transaction is rolled back.
        /// </summary>
        public event Action OnRollback
        {
            add { _rollback += value; }
            remove { _rollback -= value; }
        }
    }
}
