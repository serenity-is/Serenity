﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Serenity.Core;
#if COREFX || NET46
using Microsoft.Extensions.Configuration;
#else
using System.Configuration;
#endif


namespace Serenity.Data
{
#if COREFX || NET46
    public static class DbProviderFactories
    {
        internal static readonly Dictionary<string, Func<DbProviderFactory>> _configs = new Dictionary<string, Func<DbProviderFactory>>();

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (_configs.ContainsKey(providerInvariantName))
                return _configs[providerInvariantName]();

            throw new ArgumentOutOfRangeException("providerInvariantName");
        }

        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            _configs[providerInvariantName] = () => factory;
        }
    }
#endif

    public static class SqlConnections
    {
        private static Dictionary<string, ConnectionStringInfo> connections = new Dictionary<string, ConnectionStringInfo>();
        private static Dictionary<string, DbProviderFactory> factories = new Dictionary<string, DbProviderFactory>();

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

        public static ConnectionStringInfo TryGetConnectionString(string connectionKey)
        {
            ConnectionStringInfo connection;
            if (!connections.TryGetValue(connectionKey, out connection))
            {
                var newConnections = new Dictionary<string, ConnectionStringInfo>(connections);
#if COREFX|| NET46
                var configuration = Dependency.TryResolve<IConfiguration>();
                if (configuration == null)
                    return null;

                var connectionString = configuration.GetValue<string>("Data:" + connectionKey + ":ConnectionString");
                var providerName = configuration.GetValue<string>("Data:" + connectionKey + ":ProviderName") ?? "System.Data.SqlClient";
                connection = newConnections[connectionKey] = new ConnectionStringInfo(connectionKey, 
                    connectionString, providerName);
#else
                var connectionSetting = ConfigurationManager.ConnectionStrings[connectionKey];
                if (connectionSetting == null)
                    return null;

                connection = newConnections[connectionKey] = new ConnectionStringInfo(connectionKey, connectionSetting.ConnectionString, 
                    connectionSetting.ProviderName);
#endif

                connections = newConnections;
            }

            return connection;
        }

        public static string GetDatabaseName(string connectionKey)
        {
            var connection = TryGetConnectionString(connectionKey);
            if (connection != null)
                return connection.DatabaseName;

            return null;
        }

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
        ///   Varsayılan bağlantı string'ine göre yeni bir <see cref="DbConnection"/> nesnesi oluşturur.</summary>
        /// <returns>
        ///   Oluşturulan <see cref="DbConnection"/> nesnesi</returns>
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

        public static IDbConnection NewFor<TClass>()
        {
            var attr = typeof(TClass).GetCustomAttribute<ConnectionKeyAttribute>();
            if (attr == null)
                throw new ArgumentOutOfRangeException("Type has no ConnectionKey attribute!", typeof(TClass).FullName);

            return NewByKey(attr.Value);
        }

        public static void SetConnection(string connectionKey, string connectionString, string providerName)
        {
            var newConnections = new Dictionary<string, ConnectionStringInfo>(connections);
            newConnections[connectionKey] = new ConnectionStringInfo(connectionKey, connectionString, providerName);
            connections = newConnections;
        }

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

        public static IDbTransaction GetCurrentActualTransaction(this IDbConnection connection)
        {
            var wrapped = connection as WrappedConnection;
            if (wrapped != null && wrapped.CurrentTransaction != null)
                return wrapped.CurrentTransaction.ActualTransaction;

            return null;
        }

        public static ISqlDialect GetDialect(this IDbConnection connection)
        {
            var wrapped = connection as WrappedConnection;
            if (wrapped == null)
                return SqlSettings.DefaultDialect;

            return wrapped.Dialect ?? SqlSettings.DefaultDialect;
        }
    }
}