namespace Serenity.Data;

/// <summary>
/// Connection string setting
/// </summary>
public class ConnectionStringEntry
{
    /// <summary>
    /// Gets / sets connection string
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// Gets / sets provider name
    /// </summary>
    public string ProviderName { get; set; }
    /// <summary>
    /// Gets / sets dialect name
    /// </summary>
    public string Dialect { get; set; }

    /// <summary>
    /// Gets / sets the dialect instance
    /// </summary>
    public ISqlDialect DialectInstance { get; set; }
}