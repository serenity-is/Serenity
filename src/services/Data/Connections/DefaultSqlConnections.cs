using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// The default connection factory.
/// </summary>
/// <remarks>
/// Creates a new instance.
/// </remarks>
/// <param name="connectionStrings">The named connection strings.</param>
/// <param name="profiler">The profiler, if any.</param>
/// <param name="loggerFactory">The optional logger factory (to be used by static SqlHelper methods).</param>
public class DefaultSqlConnections(IConnectionStrings connectionStrings, IConnectionProfiler profiler = null, ILoggerFactory loggerFactory = null)
    : ISqlConnections, IConnectionKeyFallbacks
{
    /// <summary>The connection strings.</summary>
    protected readonly IConnectionStrings connectionStrings = connectionStrings ?? throw new ArgumentNullException(nameof(connectionStrings));
    /// <summary>The connection key fallbacks, if the connection string source supports them.</summary>
    protected readonly IConnectionKeyFallbacks connectionKeyFallbacks = connectionStrings as IConnectionKeyFallbacks;
    /// <summary>The profiler.</summary>
    protected readonly IConnectionProfiler profiler = profiler;
    /// <summary>The logger factory.</summary>
    protected readonly ILoggerFactory loggerFactory = loggerFactory;

    /// <summary>
    /// Lists all known connection strings.
    /// </summary>
    /// <returns>The list of all registered connections.</returns>
    public IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return connectionStrings.ListConnectionStrings();
    }

    /// <summary>
    /// Creates an actual connection based on the provider name. This should not return a wrapped connection.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>A new <see cref="IDbConnection"/> object.</returns>
    protected virtual IDbConnection CreateConnection(string connectionString, string providerName, ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(providerName);

        ArgumentNullException.ThrowIfNull(connectionString);

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

        return connection;
    }

    /// <summary>
    /// Wraps and profiles the actual connection.
    /// </summary>
    /// <param name="connection">The actual connection.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The wrapped connection.</returns>
    protected virtual IDbConnection WrapConnection(IDbConnection connection, string providerName, ISqlDialect dialect)
    {
        if (profiler != null)
            return new WrappedConnection(profiler.Profile(connection), dialect, loggerFactory?.CreateLogger<ISqlConnections>());

        return new WrappedConnection(connection, dialect, loggerFactory?.CreateLogger<ISqlConnections>());
    }

    /// <summary>
    /// Creates a new <see cref="IDbConnection"/> for the given connection string, provider name, and dialect.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>A new <see cref="IDbConnection"/> object.</returns>
    public virtual IDbConnection New(string connectionString, string providerName, ISqlDialect dialect)
    {
        var connection = CreateConnection(connectionString, providerName, dialect);
        return WrapConnection(connection, providerName, dialect);
    }

    /// <summary>
    /// Creates a new <see cref="IDbConnection"/> for the given connection key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>A new <see cref="IDbConnection"/> object.</returns>
    public virtual IDbConnection NewByKey(string connectionKey)
    {
        var info = connectionStrings.TryGetConnectionString(connectionKey) ?? throw new InvalidOperationException(string.Format("No connection string with key {0} in configuration file!", connectionKey));
        return New(info.ConnectionString, info.ProviderName, info.Dialect);
    }

    /// <summary>
    /// Gets a connection string by its key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection string, or <c>null</c> if not found.</returns>
    public virtual IConnectionString TryGetConnectionString(string connectionKey)
    {
        return connectionStrings.TryGetConnectionString(connectionKey);
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetConnectionKeyFallbacks(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);
        return connectionKeyFallbacks?.GetConnectionKeyFallbacks(connectionKey) ?? [connectionKey];
    }

    /// <inheritdoc/>
    public string ResolveConnectionKey(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);
        return connectionKeyFallbacks?.ResolveConnectionKey(connectionKey) ??
               (connectionStrings.TryGetConnectionString(connectionKey) != null ? connectionKey : null);
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetConnectionKeysResolvingTo(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);
        return connectionKeyFallbacks?.GetConnectionKeysResolvingTo(connectionKey) ??
               (connectionStrings.TryGetConnectionString(connectionKey) != null ? [connectionKey] : []);
    }
}