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
}