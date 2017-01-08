using System;
using System.Data;

namespace Serenity.Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private Action _commit;
        private Action _rollback;

        public UnitOfWork(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
            connection.EnsureOpen();
            _transaction = connection.BeginTransaction();
        }

        public IDbConnection Connection
        {
            get { return _connection; }
        }

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

        public event Action OnCommit
        {
            add { _commit += value; }
            remove { _commit -= value; }
        }

        public event Action OnRollback
        {
            add { _rollback += value; }
            remove { _rollback -= value; }
        }
    }
}
