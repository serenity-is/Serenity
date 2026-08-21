using Microsoft.Extensions.Options;

namespace Serenity.Data;

/// <summary>
/// The default connection string source.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="DefaultConnectionStrings"/>.
/// </remarks>
/// <param name="options">The connection string options.</param>
/// <param name="sqlDialectMapper">The SQL dialect mapper.</param>
public class DefaultConnectionStrings(IOptions<ConnectionStringOptions> options, ISqlDialectMapper sqlDialectMapper = null) : IConnectionStrings
{
    /// <summary>The options.</summary>
    protected readonly IOptions<ConnectionStringOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    /// <summary>The SQL dialect mapper.</summary>
    protected readonly ISqlDialectMapper sqlDialectMapper = sqlDialectMapper ?? new DefaultSqlDialectMapper();
    /// <summary>The cached dictionary of connection string infos.</summary>
    protected readonly ConcurrentDictionary<string, ConnectionStringInfo> byKey = new();

    /// <summary>
    /// Determines the dialect for a connection.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <param name="entry">The connection entry.</param>
    /// <returns>The SQL dialect.</returns>
    protected virtual ISqlDialect DetermineDialect(string connectionKey, ConnectionStringEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.DialectInstance != null)
            return entry.DialectInstance;
        
        if (string.IsNullOrEmpty(entry.Dialect))
            return sqlDialectMapper.TryGet(entry.ProviderName) ?? SqlSettings.DefaultDialect;
        
        return sqlDialectMapper.TryGet(entry.Dialect) ?? 
            throw new ArgumentException($"Dialect type {entry.Dialect} specified for connection {connectionKey} is not found!");
    }

    /// <summary>
    /// Gets a connection string by its key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection string, or <c>null</c> if not found.</returns>
    public virtual IConnectionString TryGetConnectionString(string connectionKey)
    {
        if (byKey.TryGetValue(connectionKey, out ConnectionStringInfo info))
            return info;

        if (!options.Value.TryGetValue(connectionKey, out ConnectionStringEntry entry))
            return null;

        info = new ConnectionStringInfo(connectionKey, entry.ConnectionString, entry.ProviderName,
            DetermineDialect(connectionKey, entry));

        byKey[connectionKey] = info;
        return info;
    }

    /// <summary>
    /// Lists all known connection strings.
    /// </summary>
    /// <returns>The list of all registered connections.</returns>
    public virtual IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return options.Value.Keys.Select(TryGetConnectionString);
    }
}