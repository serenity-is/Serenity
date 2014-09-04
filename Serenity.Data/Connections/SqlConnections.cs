using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Serenity.Data
{
    public static class SqlConnections
    {
        private static Dictionary<string, Tuple<string, DbProviderFactory>> connections = new Dictionary<string, Tuple<string, DbProviderFactory>>();
        private static Dictionary<string, DbProviderFactory> factories = new Dictionary<string, DbProviderFactory>();
        
        /// <summary>
        ///   Uygulamanın varsayılan bağlantı string'ine erişimde kullanılan isim.</summary>
        public const string DefaultConnectionKey = "Default";

        public static DbProviderFactory GetFactory(string providerName)
        {
            DbProviderFactory factory;
            if (!factories.TryGetValue(providerName, out factory))
            {
                var newFactories = new Dictionary<string, DbProviderFactory>(factories);
                factory = newFactories[providerName] = DbProviderFactories.GetFactory(providerName);
                factories = newFactories;
            }

            return factory;
        }

        public static Tuple<string, DbProviderFactory> GetConnectionString(string connectionKey)
        {
            Tuple<string, DbProviderFactory> connection;
            if (!connections.TryGetValue(connectionKey, out connection))
            {
                var newConnections = new Dictionary<string, Tuple<string, DbProviderFactory>>(connections);
                var connectionSetting = ConfigurationManager.ConnectionStrings[connectionKey];
                if (connectionSetting == null)
                    throw new InvalidOperationException(String.Format("No connection string with key {0} in configuration file!", connectionKey));

                var factory = GetFactory(connectionSetting.ProviderName);

                connection = newConnections[connectionKey] = new Tuple<string, DbProviderFactory>(connectionSetting.ConnectionString, factory);
                connections = newConnections;
            }

            return connection;
        }

        /// <summary>
        ///   Bağlantı string'ine göre yeni bir <see cref="DbConnection"/> nesnesi oluşturur.</summary>
        /// <param name="connectionString">
        ///   Oluşturulacak bağlantı string'i</param>
        /// <returns>
        ///   Oluşturulan <see cref="DbConnection"/> nesnesi</returns>
        public static IDbConnection New(string connectionString, string providerName)
        {
            var factory = GetFactory(providerName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return new WrappedConnection(connection);
        }

        /// <summary>
        ///   Varsayılan bağlantı string'ine göre yeni bir <see cref="DbConnection"/> nesnesi oluşturur.</summary>
        /// <returns>
        ///   Oluşturulan <see cref="DbConnection"/> nesnesi</returns>
        public static IDbConnection New()
        {
            var connectionSetting = GetConnectionString(DefaultConnectionKey);
            var connection = connectionSetting.Item2.CreateConnection();
            connection.ConnectionString = connectionSetting.Item1;
            return connection;
        }


        /// <summary>
        ///   Varsayılan bağlantı string'ine göre yeni bir <see cref="DbConnection"/> nesnesi oluşturur.</summary>
        /// <returns>
        ///   Oluşturulan <see cref="DbConnection"/> nesnesi</returns>
        public static IDbConnection NewByKey(string connectionKey)
        {
            var connectionSetting = GetConnectionString(connectionKey);
            var connection = connectionSetting.Item2.CreateConnection();
            connection.ConnectionString = connectionSetting.Item1;
            return new WrappedConnection(connection);
        }

        public static void SetConnection(string connectionKey, string connectionString, string providerName)
        {
            var newConnections = new Dictionary<string, Tuple<string, DbProviderFactory>>(connections);
            newConnections[connectionKey] = new Tuple<string, DbProviderFactory>(connectionString, GetFactory(providerName));
            connections = newConnections;
        }

        public static IDbConnection EnsureOpen(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (connection.State != ConnectionState.Open)
                connection.Open();

            return connection;
        }

        public static IDbTransaction GetCurrentTransaction(this IDbConnection connection)
        {
            var wrapped = connection as IWrappedConnection;
            if (wrapped != null)
                return wrapped.Transaction;

            return null;
        }
    }
}