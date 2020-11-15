using System;
using System.Data;
using System.Data.Common;

namespace Serenity.Data
{
    /// <summary>
    /// Default connection factory
    /// </summary>
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConnectionStrings connectionStrings;
        private readonly IConnectionProfiler profiler;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="connectionStrings">Named connection strings</param>
        /// <param name="profiler">Profiler if any</param>
        public ConnectionFactory(IConnectionStrings connectionStrings, IConnectionProfiler profiler = null)
        {
            this.connectionStrings = connectionStrings ?? throw new ArgumentNullException(nameof(connectionStrings));
            this.profiler = profiler;
        }

        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> for given connection string, provider name and dialect.</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="dialect">Dialect</param>
        /// <returns>A new <see cref="IDbConnection"/> object.</returns>
        public virtual IDbConnection New(string connectionString, string providerName, ISqlDialect dialect)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var factory = DbProviderFactories.GetFactory(providerName);
            var connection = factory.CreateConnection();
            try
            {
                connection.ConnectionString = connectionString;
            }
            catch
            {
                connection.Dispose();
                return null;
            }

            if (profiler != null)
                return new WrappedConnection(profiler.Profile(connection), dialect);

            return new WrappedConnection(connection, dialect);
        }

        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> for given connection key.</summary>
        /// <param name="connectionKey">Connection key</param>
        /// <returns>A new <see cref="IDbConnection"/> object.</returns>
        public virtual IDbConnection NewByKey(string connectionKey)
        {
            var info = connectionStrings.TryGet(connectionKey);

            if (info == null)
                throw new InvalidOperationException(string.Format("No connection string with key {0} in configuration file!", connectionKey));

            return New(info.ConnectionString, info.ProviderName, info.Dialect);
        }
    }
}