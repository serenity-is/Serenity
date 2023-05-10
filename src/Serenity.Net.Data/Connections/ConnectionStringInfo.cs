namespace Serenity.Data;

/// <summary>
/// Contains a connection string, its key and provider name.
/// </summary>
public class ConnectionStringInfo : IConnectionString
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringInfo"/> class.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <param name="dialect">Dialect</param>
    public ConnectionStringInfo(string connectionKey, string connectionString, string providerName, ISqlDialect dialect)
    {
        ConnectionKey = connectionKey ?? throw new ArgumentNullException(nameof(connectionKey));
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
        Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
    }

    /// <summary>
    /// Gets the dialect.
    /// </summary>
    public ISqlDialect Dialect { get; private set; }

    /// <summary>
    /// Gets the connection key.
    /// </summary>
    /// <value>
    /// The connection key.
    /// </value>
    public string ConnectionKey { get; private set; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <value>
    /// The connection string.
    /// </value>
    public string ConnectionString { get; private set; }

    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    /// <value>
    /// The name of the provider.
    /// </value>
    public string ProviderName { get; private set; }
}