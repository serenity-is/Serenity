namespace Serenity.Data;

/// <summary>
/// Contains connection string extensions
/// </summary>
public static class ConnectionStringsExtensions
{
    /// <summary>
    /// Gets connection string by key
    /// </summary>
    /// <param name="connectionStrings">Connection strings object</param>
    /// <param name="connectionKey">Connection key</param>
    /// <returns>Connection string with key, or throws an ArgumentOutOfRangeException</returns>
    public static IConnectionString Get(this IConnectionStrings connectionStrings, string connectionKey)
    {
        if (connectionStrings == null)
            throw new ArgumentNullException(nameof(connectionStrings));

        var connectionString = connectionStrings.TryGetConnectionString(connectionKey);
        if (connectionString == null)
            throw new ArgumentOutOfRangeException(nameof(connectionKey));

        return connectionString;
    }
}