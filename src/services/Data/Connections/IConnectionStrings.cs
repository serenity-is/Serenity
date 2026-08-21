namespace Serenity.Data;

/// <summary>
/// Abstraction to access connection strings along with dialect and provider information.
/// </summary>
public interface IConnectionStrings
{
    /// <summary>
    /// Gets a connection string by its key.
    /// </summary>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection string, or <c>null</c> if not found.</returns>
    IConnectionString TryGetConnectionString(string connectionKey);

    /// <summary>
    /// Lists all known connection strings.
    /// </summary>
    /// <returns>The list of all registered connections.</returns>
    IEnumerable<IConnectionString> ListConnectionStrings();
}