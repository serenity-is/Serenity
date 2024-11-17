namespace Serenity.Data;

/// <summary>
/// Contains a connection string, its key and provider name.
/// </summary>
public interface IConnectionString
{
    /// <summary>
    /// Gets the dialect.
    /// </summary>
    /// <value>
    /// The dialect.
    /// </value>
    ISqlDialect Dialect { get; }

    /// <summary>
    /// Gets the connection key.
    /// </summary>
    /// <value>
    /// The connection key.
    /// </value>
    string ConnectionKey { get; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <value>
    /// The connection string.
    /// </value>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    /// <value>
    /// The name of the provider.
    /// </value>
    string ProviderName { get; }
}