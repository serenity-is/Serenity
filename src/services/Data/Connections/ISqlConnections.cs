namespace Serenity.Data;

/// <summary>
/// An interface used to abstract SQL connections.
/// </summary>
public interface ISqlConnections : IConnectionStrings
{
    /// <summary>
    /// Creates a new <see cref="IDbConnection"/> for the given connection string, provider name, and dialect.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>A new <see cref="IDbConnection"/> object.</returns>
    IDbConnection New(string connectionString, string providerName, ISqlDialect dialect);

    /// <summary>
    /// Creates a new <see cref="IDbConnection"/> for the given connection key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>A new <see cref="IDbConnection"/> object.</returns>
    IDbConnection NewByKey(string connectionKey);
}