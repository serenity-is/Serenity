namespace Serenity.Data;

/// <summary>
/// Abstraction to access connection strings along with dialect and provider information
/// </summary>
public interface IConnectionStrings
{
    /// <summary>
    /// Gets a connection string by its key
    /// </summary>
    /// <param name="connectionKey">Connection key</param>
    /// <returns>Connection string or null if not found</returns>
    IConnectionString TryGetConnectionString(string connectionKey);

    /// <summary>
    /// Lists all known connections strings
    /// </summary>
    /// <returns>List of all registered connections</returns>
    IEnumerable<IConnectionString> ListConnectionStrings();
}