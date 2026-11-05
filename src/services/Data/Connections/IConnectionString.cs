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

    /// <summary>
    /// Gets the default collation to use if the database for this connection
    /// is created by the application, or null to use the default of the server.
    /// </summary>
    string? Collation => null;

    /// <summary>
    /// Gets the comparer specification of this connection, if any.
    /// </summary>
    string? Comparer => null;

    /// <summary>
    /// Gets a value indicating whether a SQL Server database for this connection
    /// is created as a partially contained database, if it is created by the application.
    /// </summary>
    bool PartialContainment => false;
}