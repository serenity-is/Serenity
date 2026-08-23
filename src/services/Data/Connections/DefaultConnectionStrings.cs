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
/// <param name="typeSource">The type source used to discover connection key fallbacks.</param>
public class DefaultConnectionStrings(IOptions<ConnectionStringOptions> options, ISqlDialectMapper sqlDialectMapper = null, ITypeSource typeSource = null)
    : IConnectionStrings, IConnectionKeyFallbacks
{
    /// <summary>The options.</summary>
    protected readonly IOptions<ConnectionStringOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    /// <summary>The SQL dialect mapper.</summary>
    protected readonly ISqlDialectMapper sqlDialectMapper = sqlDialectMapper ?? new DefaultSqlDialectMapper();
    /// <summary>The cached dictionary of connection string infos.</summary>
    protected readonly ConcurrentDictionary<string, ConnectionStringInfo> byKey = new();
    /// <summary>The lazily built connection key fallback map.</summary>
    /// <remarks>
    /// Built on first access via <see cref="GetFallbackMap"/>. A derived implementation can
    /// override <see cref="GetFallbackMap"/> to avoid caching, or set this field to
    /// <c>null</c> to invalidate it so it is rebuilt on the next access.
    /// </remarks>
    protected Dictionary<string, string> fallbackMap;

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
    /// Gets a connection string by its key, resolving any connection key fallbacks.
    /// If the specified key is not configured but has a fallback, the fallback connection
    /// string is returned. The returned <see cref="IConnectionString.ConnectionKey"/> is
    /// the key the connection is actually registered under.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection string, or <c>null</c> if not found.</returns>
    public virtual IConnectionString TryGetConnectionString(string connectionKey)
    {
        if (byKey.TryGetValue(connectionKey, out ConnectionStringInfo info))
            return info;

        var resolvedKey = ResolveConnectionKey(connectionKey);
        if (resolvedKey == null)
            return null;

        if (byKey.TryGetValue(resolvedKey, out info))
            return info;

        if (!options.Value.TryGetValue(resolvedKey, out ConnectionStringEntry entry))
            return null;

        info = new ConnectionStringInfo(resolvedKey, entry.ConnectionString, entry.ProviderName,
            DetermineDialect(resolvedKey, entry));

        byKey[resolvedKey] = info;
        return info;
    }

    /// <summary>
    /// Lists all known connection strings.
    /// </summary>
    /// <returns>The list of all registered connections.</returns>
    /// <remarks>
    /// Only returns connection strings that are actually configured. Connection keys that
    /// are only resolved through a <see cref="ConnectionKeyFallbackAttribute"/> fallback are
    /// not included unless they are also configured.
    /// </remarks>
    public virtual IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return options.Value.Keys.Select(TryGetConnectionString);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<string> GetConnectionKeyFallbacks(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);

        var map = GetFallbackMap();
        var result = new List<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var current = connectionKey;
        while (current != null)
        {
            if (!visited.Add(current))
                throw new InvalidOperationException(
                    "Connection key fallback cycle detected: " +
                    string.Join(" -> ", result.Append(current)));

            result.Add(current);
            if (!map.TryGetValue(current, out var next))
                break;
            current = next;
        }
        return result;
    }

    /// <inheritdoc/>
    public virtual string ResolveConnectionKey(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);

        foreach (var key in GetConnectionKeyFallbacks(connectionKey))
        {
            if (options.Value.ContainsKey(key))
                return key;
        }
        return null;
    }

    /// <summary>
    /// Returns all connection keys (including themselves) whose fallback chain resolves to
    /// the specified connection key. Only keys that actually resolve to a <em>configured</em>
    /// connection are included, so this is a configuration-aware operation.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection keys resolving to the specified key.</returns>
    public virtual IEnumerable<string> GetConnectionKeysResolvingTo(string connectionKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionKey);

        var map = GetFallbackMap();
        var result = new List<string>();
        var candidates = new HashSet<string>(options.Value.Keys, StringComparer.OrdinalIgnoreCase);
        foreach (var key in map.Keys)
            candidates.Add(key);

        foreach (var candidate in candidates)
        {
            if (string.Equals(ResolveConnectionKey(candidate), connectionKey, StringComparison.OrdinalIgnoreCase))
                result.Add(candidate);
        }
        return result;
    }

    /// <summary>
    /// Gets the connection key fallback map. The default implementation builds it lazily on
    /// first access and caches it in <see cref="fallbackMap"/>. Override to change caching
    /// or invalidation behavior, e.g. to return a fresh map each access (no caching).
    /// </summary>
    protected virtual Dictionary<string, string> GetFallbackMap()
        => fallbackMap ??= BuildFallbackMap(typeSource);

    /// <summary>
    /// Builds the connection key fallback map from the given type source.
    /// </summary>
    /// <param name="typeSource">The type source used to discover connection key fallbacks.</param>
    /// <returns>The connection key fallback map.</returns>
    protected static Dictionary<string, string> BuildFallbackMap(ITypeSource typeSource)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (typeSource == null)
            return map;

        // The application assembly typically appears last in the type source, so a later
        // assembly's fallback overrides an earlier one, allowing applications to override
        // framework defaults.
        foreach (ConnectionKeyFallbackAttribute attr in typeSource.GetAssemblyAttributes(typeof(ConnectionKeyFallbackAttribute)))
        {
            map[attr.ConnectionKey] = attr.FallbackConnectionKey;
        }
        return map;
    }
}