using System.Data;

namespace Serenity.Data
{
    /// <summary>
    /// An interface used to abstract connection factories
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> for given connection string, provider name and dialect.</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="dialect">Dialect</param>
        /// <returns>A new <see cref="IDbConnection"/> object.</returns>
        IDbConnection New(string connectionString, string providerName, ISqlDialect dialect);

        /// <summary>
        /// Creates a new <see cref="IDbConnection"/> for given connection key.</summary>
        /// <param name="connectionKey">Connection key</param>
        /// <returns>A new <see cref="IDbConnection"/> object.</returns>
        IDbConnection NewByKey(string connectionKey);
    }
}