namespace Serenity.Testing;

public abstract class MemoryDbBase : ISqlConnections, IDisposable, IConnectionKeyFallbacks
{
    private bool disposed;
    private readonly DefaultSqlConnections sqlConnections;
    protected readonly Dictionary<string, MemoryDbInstance> byKey;

    public MemoryDbBase()
    {
        sqlConnections = new DefaultSqlConnections(this);
        byKey = new(StringComparer.OrdinalIgnoreCase);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                foreach (var instance in byKey.Values)
                    instance.Dispose();
                byKey.Clear();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void CreateSnapshots()
    {
        foreach (var instance in byKey.Values)
            instance.CreateSnapshot();
    }

    public void RestoreSnapshots()
    {
        foreach (var instance in byKey.Values)
            instance.RestoreSnapshot();
    }

    public IConnectionString TryGetConnectionString(string connectionKey)
    {
        if (!byKey.TryGetValue(connectionKey, out MemoryDbInstance instance))
        {
            instance = null;
            foreach (var pair in byKey)
                if (pair.Value.FallbackFor.Contains(connectionKey))
                {
                    instance = pair.Value;
                    break;
                }
            if (instance == null)
                return null;
        }

        return new ConnectionStringInfo(connectionKey, instance.ConnectionString,
            "Microsoft.Data.Sqlite", SqliteDialect.Instance);
    }

    public IDbConnection NewByKey(string connectionKey)
    {
        if (!byKey.TryGetValue(connectionKey, out MemoryDbInstance instance))
        {
            return sqlConnections.NewByKey(connectionKey);
        }

        return instance.AttachSchemasOnOpen(sqlConnections.NewByKey(connectionKey));
    }

    public IDbConnection New(string connectionString, string providerName, ISqlDialect dialect)
    {
        var instance = byKey.Values.FirstOrDefault(x => string.Equals(x.ConnectionString, connectionString,
            StringComparison.OrdinalIgnoreCase));

        if (instance != null)
            return instance.AttachSchemasOnOpen(sqlConnections.New(connectionString, providerName, dialect));

        return sqlConnections.New(connectionString, providerName, dialect);
    }

    public IEnumerable<IConnectionString> ListConnectionStrings()
    {
        return byKey.Select(x => new ConnectionStringInfo(x.Key,
            x.Value.ConnectionString, "Microsoft.Data.Sqlite", SqliteDialect.Instance));
    }

    public MemoryDbInstance GetDbInstanceByKey(string key)
    {
        if (!byKey.TryGetValue(key, out MemoryDbInstance instance))
            return null;

        return instance;
    }

    private Dictionary<string, string> GetFallbackMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, instance) in byKey)
        {
            foreach (var source in instance.FallbackFor)
                map[source] = key;
        }
        return map;
    }

    public IEnumerable<string> GetConnectionKeyFallbacks(string connectionKey)
    {
        var map = GetFallbackMap();
        var result = new List<string>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var current = connectionKey;
        while (current != null && visited.Add(current))
        {
            result.Add(current);
            if (!map.TryGetValue(current, out var next))
                break;
            current = next;
        }
        return result;
    }

    public string ResolveConnectionKey(string connectionKey)
    {
        foreach (var key in GetConnectionKeyFallbacks(connectionKey))
        {
            if (byKey.ContainsKey(key))
                return key;
        }
        return null;
    }

    public IEnumerable<string> GetConnectionKeysResolvingTo(string connectionKey)
    {
        var result = new List<string>();
        var candidates = new HashSet<string>(byKey.Keys, StringComparer.OrdinalIgnoreCase);
        foreach (var key in GetFallbackMap().Keys)
            candidates.Add(key);

        foreach (var candidate in candidates)
        {
            if (string.Equals(ResolveConnectionKey(candidate), connectionKey, StringComparison.OrdinalIgnoreCase))
                result.Add(candidate);
        }
        return result;
    }

    public void AddFallbacksFrom(Assembly[] assemblies)
    {
        var typeSource = new DefaultTypeSource(assemblies);
        var opt = new ConnectionStringOptions();
        foreach (var pair in byKey)
        {
            opt[pair.Key] = new()
            {
                ConnectionString = pair.Value.ConnectionString
            };
        }
        var connectionStrings = new DefaultConnectionStrings(opt, typeSource: typeSource);
        foreach (var pair in byKey)
        {
            var fallbackList = connectionStrings.GetConnectionKeysResolvingTo(pair.Key);
            pair.Value.FallbackFor.AddRange(fallbackList.Where(x => !string.Equals(x, pair.Key, StringComparison.OrdinalIgnoreCase)));
        }
    }
}