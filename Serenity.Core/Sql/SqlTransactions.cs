using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Serenity.Data
{
    public static class SqlTransactions
    {
        /// <summary>
        ///   Gets current IDbTransaction for a connection, that is started with DataHelper.BeginTransaction method.</summary>
        /// <param name="connection">
        ///   Connection (required)</param>
        /// <returns>
        ///   The automatic transaction associated with the connection.</returns>
        private static IDbTransaction GetDefaultTransaction(IDbConnection connection)
        {
            if (connection == null)
                return null;

            return ContextItems.Get(connection) as IDbTransaction;
        }

        /// <summary>
        ///   Gets current (auto) DbTransaction for a connection, that is started with DataHelper.BeginTransaction 
        ///   method.</summary>
        /// <param name="connection">
        ///   Connection (required)</param>
        /// <returns>
        ///   The automatic transaction associated with the connection.</returns>
        internal static DbTransaction GetCurrentTransaction(IDbConnection connection)
        {
            return GetDefaultTransaction(connection) as DbTransaction;
        }

        /// <summary>
        ///   Sets current (auto) IDbTransaction for a connection.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <param name="transaction">
        ///   Default (auto) transaction for the connection.</param>
        internal static void SetDefaultTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            if (connection == null)
                return;
            ContextItems.Set(connection, transaction);
        }

        /// <summary>
        ///   Starts a new transaction on connection, and sets it as default (auto) transaction for 
        ///   the connection.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <returns>
        ///   A virtual automatic transaction, that can be used to commit/rollback actual 
        ///   underlying transaction.</returns>
        public static AutoTransaction BeginTransaction(IDbConnection connection)
        {
            SqlHelper.EnsureOpen(connection);
            IDbTransaction transaction = connection.BeginTransaction();
            try
            {
                SetDefaultTransaction(connection, transaction);
                return new AutoTransaction(transaction);
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        }

        /// <summary>
        ///   Starts a new transaction on connection, and sets it as default (auto) transaction for 
        ///   the connection if the connection doesn't have an automatic transaction already started.
        ///   Returns a DummyTransaction otherwise.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <returns>
        ///   A virtual automatic transaction, that can be used to commit/rollback actual 
        ///   underlying transaction, or a dummy transaction.</returns>
        public static IDbTransaction BeginTransactionIf(IDbConnection connection)
        {
            if (GetDefaultTransaction(connection) != null)
                return new DummyTransaction();
            else
                return BeginTransaction(connection);
        }

        /// <summary>
        ///   Returns true if the connection has an active auto transaction.</summary>
        /// <param name="connection">
        ///   Connection (required).</param>
        /// <returns>
        ///   True if connection has an active auto transaction.</returns>
        public static bool IsInTransaction(IDbConnection connection)
        {
            return (GetDefaultTransaction(connection) != null);
        }
    }
}