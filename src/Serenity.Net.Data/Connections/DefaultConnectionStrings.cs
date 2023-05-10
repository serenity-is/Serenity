using Microsoft.Extensions.Options;

namespace Serenity.Data;

/// <summary>
/// Default connection string source
/// </summary>
public class DefaultConnectionStrings : IConnectionStrings
{
    /// <summary>Options</summary>
    protected readonly IOptions<ConnectionStringOptions> options;
    /// <summary>Sql dialect mapper</summary>
    protected readonly ISqlDialectMapper sqlDialectMapper;
    /// <summary>Cached dictionary of connection string infos</summary>
    protected readonly ConcurrentDictionary<string, ConnectionStringInfo> byKey = new();

    /// <summary>
    /// Creates a new instance of DefaultConnectionStringSource
    /// </summary>
    /// <param name="options">Connection string options</param>
    /// <param name="sqlDialectMapper">Sql Dialect Mapper</param>
    public DefaultConnectionStrings(IOptions<ConnectionStringOptions> options, ISqlDialectMapper sqlDialectMapper = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.sqlDialectMapper = sqlDialectMapper ?? new DefaultSqlDialectMapper();
    }

    /// <summary>
    /// Determines dialect for a connection
    /// </summary>
    /// <param name="connectionKey">Connection key</param>
    /// <param name="entry">Connection entry</param>
    protected virtual ISqlDialect DetermineDialect(string connectionKey, ConnectionStringEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        if (entry.DialectInstance != null)
            return entry.DialectInstance;
        
        if (string.IsNullOrEmpty(entry.Dialect))
            return sqlDialectMapper.TryGet(entry.ProviderName) ?? SqlSettings.DefaultDialect;
        
        return sqlDialectMapper.TryGet(entry.Dialect) ?? 
            throw new ArgumentException($"Dialect type {entry.Dialect} specified for connection {connectionKey} is not found!");
    }

    /// <summary>
    /// Gets a connection string by its key
    /// </summary>
    /// <param name="connectionKey">Connection key</param>
    /// <returns>Connection string or null if not found</returns>
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
    /// Lists all known connections strings
    /// </summary>
    /// <returns>List of all registered connections</returns>
    public virtual IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return options.Value.Keys.Select(x => TryGetConnectionString(x));
    }
}