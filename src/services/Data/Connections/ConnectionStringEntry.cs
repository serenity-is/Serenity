namespace Serenity.Data;

/// <summary>
/// A connection string setting.
/// </summary>
public class ConnectionStringEntry
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string ProviderName { get; set; }
    /// <summary>
    /// Gets or sets the dialect name.
    /// </summary>
    public string Dialect { get; set; }

    /// <summary>
    /// Gets or sets the dialect instance.
    /// </summary>
    public ISqlDialect DialectInstance { get; set; }

    /// <summary>
    /// Gets or sets a semicolon-separated list of connection keys that fall back to this
    /// connection when they are not configured. For example, "ProFeatures;ProWorkLog".
    /// </summary>
    /// <remarks>
    /// This is equivalent to declaring <c>[assembly: ConnectionKeyFallback(key, thisConnectionKey)]</c>
    /// for each listed key, but is driven by configuration and overrides any such assembly attribute.
    /// </remarks>
    public string FallbackFor { get; set; }

    private HashSet<string> fallbackForKeys;

    /// <summary>
    /// Gets the parsed, trimmed connection keys from <see cref="FallbackFor"/>, split on ';'.
    /// Returns an empty set when <see cref="FallbackFor"/> is null or empty. Parsed lazily and cached.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<string> FallbackForKeys
        => fallbackForKeys ??= FallbackFor is null
            ? []
            : new HashSet<string>(
                FallbackFor.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                StringComparer.OrdinalIgnoreCase);
}