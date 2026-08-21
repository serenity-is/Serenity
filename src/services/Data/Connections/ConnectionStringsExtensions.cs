namespace Serenity.Data;

/// <summary>
/// Contains connection string extensions.
/// </summary>
public static class ConnectionStringsExtensions
{
    /// <summary>
    /// Gets the connection string by key.
    /// </summary>
    /// <param name="connectionStrings">The connection strings object.</param>
    /// <param name="connectionKey">The connection key.</param>
    /// <returns>The connection string with the key, or throws an <see cref="ArgumentOutOfRangeException"/>.</returns>
    public static IConnectionString Get(this IConnectionStrings connectionStrings, string connectionKey)
    {
        ArgumentNullException.ThrowIfNull(connectionStrings);

        var connectionString = connectionStrings.TryGetConnectionString(connectionKey) ?? throw new ArgumentOutOfRangeException(nameof(connectionKey));
        return connectionString;
    }
}