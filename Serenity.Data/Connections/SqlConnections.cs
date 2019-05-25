using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Serenity.Data
{
    /// <summary>
    /// Contains DB connection related functions
    /// </summary>
    public static class SqlConnections
    {
        private static Dictionary<string, ConnectionStringInfo> connections = new Dictionary<string, ConnectionStringInfo>();
        private static Dictionary<string, DbProviderFactory> factories = new Dictionary<string, DbProviderFactory>();

        /// <summary>
        /// Gets a factory with given name
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>Provider factory</returns>
        public static DbProviderFactory GetFactory(string providerName)
        {
            DbProviderFactory factory;
            if (!factories.TryGetValue(providerName, out factory))
            {
                var newFactories = new Dictionary<string, DbProviderFactory>(factories);
                DbProviderFactories.GetFactory(providerName);
                factory = newFactories[providerName] = DbProviderFactories.GetFactory(providerName);
                factories = newFactories;
            }

            return factory;
        }

        /// <summary>
        /// Tries to get a connection string by its key.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>Connection string</returns>
        public static ConnectionStringInfo TryGetConnectionString(string connectionKey)
        {
            ConnectionStringInfo connection;
            if (!connections.TryGetValue(connectionKey, out connection))
            {
                var newConnections = new Dictionary<string, ConnectionStringInfo>(connections);

                var configuration = Dependency.TryResolve<IConfigurationManager>();
                if (configuration == null)
                    return null;

                var connectionSetting = configuration.ConnectionString(connectionKey);
                if (connectionSetting == null)
                    return null;

                connection = newConnections[connectionKey] = new ConnectionStringInfo(connectionKey, connectionSetting.Item1, 
                    connectionSetting.Item2);

                connections = newConnections;
            }

            return connection;
        }

        /// <summary>
        /// Gets the name of database for a specified connection key by parsing it.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>Database name, or null if can't be parsed.</returns>
        public static string GetDatabaseName(string connectionKey)
        {
            var connection = TryGetConnectionString(connectionKey);
            if (connection != null)
                return connection.DatabaseName;

            return null;
        }

        /// <summary>
        /// Gets the connection string for a specified connection key.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>Connection string</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static ConnectionStringInfo GetConnectionString(string connectionKey)
        {
            var connection = TryGetConnectionString(connectionKey);
            if (connection == null)
                throw new InvalidOperationException(String.Format("No connection string with key {0} in configuration file!", connectionKey));

            return connection;
        }

        /// <summary>
        ///   Creates a new <see cref="DbConnection"/> for given connection string and provider name.</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="providerName">Provider name</param>
        /// <returns>A new <see cref="DbConnection"/> object.</returns>
        public static IDbConnection New(string connectionString, string providerName)
        {
            var factory = GetFactory(providerName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            var dialect = ConnectionStringInfo.GetDialectByProviderName(providerName) ?? SqlSettings.DefaultDialect;

            var profiler = Dependency.TryResolve<IConnectionProfiler>();
            if (profiler != null)
                return new WrappedConnection(profiler.Profile(connection), dialect);

            return new WrappedConnection(connection, dialect);
        }

        /// <summary>
        /// Creates a new connection for specified connection key.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <returns>A new connection</returns>
        public static IDbConnection NewByKey(string connectionKey)
        {
            var connectionSetting = GetConnectionString(connectionKey);
            var connection = connectionSetting.ProviderFactory.CreateConnection();
            connection.ConnectionString = connectionSetting.ConnectionString;

            var profiler = Dependency.TryResolve<IConnectionProfiler>();
            if (profiler != null)
                return new WrappedConnection(profiler.Profile(connection), connectionSetting.Dialect);

            return new WrappedConnection(connection, connectionSetting.Dialect);
        }

        /// <summary>
        /// Creates a new connection for specified class, determining 
        /// the connection key by checking its [ConnectionKey] attribute.
        /// </summary>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <returns>A new connection</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Type has no ConnectionKey attribute!</exception>
        public static IDbConnection NewFor<TClass>()
        {
            var attr = typeof(TClass).GetCustomAttribute<ConnectionKeyAttribute>();
            if (attr == null)
                throw new ArgumentOutOfRangeException("Type has no ConnectionKey attribute!", typeof(TClass).FullName);

            return NewByKey(attr.Value);
        }

        /// <summary>
        /// Sets a connection string. 
        /// Warning! This is not a thread save method and is only intended for specific cases like unit tests,
        /// or to determine a connection string at application start. Don't try to implement dynamic connection
        /// strings or multi tenancy with this one! In a web environment, context switches or parallel requests
        /// might occur and your request/data/sql may end up in a different connection than the original one
        /// you set at request start!
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void SetConnection(string connectionKey, string connectionString, string providerName)
        {
            var newConnections = new Dictionary<string, ConnectionStringInfo>(connections);
            newConnections[connectionKey] = new ConnectionStringInfo(connectionKey, connectionString, providerName);
            connections = newConnections;
        }

        /// <summary>
        /// Ensures the connection is open. Warning! This method will not reopen a connection that once was opened
        /// and will raise an error.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        /// <exception cref="System.InvalidOperationException">Can't auto open a closed connection that was previously open!</exception>
        public static IDbConnection EnsureOpen(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (connection.State != ConnectionState.Open)
            {
                var wrapped = connection as WrappedConnection;
                if (wrapped != null && wrapped.OpenedOnce)
                    throw new InvalidOperationException("Can't auto open a closed connection that was previously open!");

                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Gets the current actual transaction for a connection if any.
        /// Most of the time, a connection will only have one transaction, 
        /// but in .NET it is not possible to know what is that transaction.
        /// Serenity wraps a connection (WrappedConnection) so that running
        /// transaction if any is available to get from the connection object.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The current transaction for a connection</returns>
        public static IDbTransaction GetCurrentActualTransaction(this IDbConnection connection)
        {
            var wrapped = connection as WrappedConnection;
            if (wrapped != null && wrapped.CurrentTransaction != null)
                return wrapped.CurrentTransaction.ActualTransaction;

            return null;
        }

        public static void SetCommandTimeout(this IDbConnection connection, int? value)
        {
            var wrapped = connection as WrappedConnection;
            if (wrapped != null)
                wrapped.CommandTimeout = value;
            else
                throw new ArgumentOutOfRangeException(nameof(connection));
        }

        /// <summary>
        /// Gets the dialect for given connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The sql dialect.</returns>
        public static ISqlDialect GetDialect(this IDbConnection connection)
        {
            var wrapped = connection as WrappedConnection;
            if (wrapped == null)
                return SqlSettings.DefaultDialect;

            return wrapped.Dialect ?? SqlSettings.DefaultDialect;
        }
    }
}