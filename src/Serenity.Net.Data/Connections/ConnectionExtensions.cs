using Microsoft.Extensions.Logging;

namespace Serenity.Data;

/// <summary>
/// Contains DB connection related extensions
/// </summary>
public static class ConnectionExtensions
{
    /// <summary>
    /// Default connection key, this is an optional name
    /// </summary>
    public const string DefaultConnectionKey = "Default";

    /// <summary>
    /// Creates a new connection for specified class, determining 
    /// the connection key by checking its [ConnectionKey] attribute.
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <param name="factory">Connection factory</param>
    /// <returns>A new connection</returns>
    /// <exception cref="ArgumentOutOfRangeException">Type has no ConnectionKey attribute!</exception>
    public static IDbConnection NewFor<TClass>(this ISqlConnections factory)
    {
        var attr = typeof(TClass).GetCustomAttribute<ConnectionKeyAttribute>();
        if (attr == null)
            throw new ArgumentOutOfRangeException("Type has no ConnectionKey attribute!", typeof(TClass).FullName);

        return factory.NewByKey(attr.Value);
    }

    /// <summary>
    /// Ensures the connection is open. Warning! This method will not reopen a connection that once was opened
    /// and will raise an error.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">connection</exception>
    /// <exception cref="InvalidOperationException">Can't auto open a closed connection that was previously open!</exception>
    public static IDbConnection EnsureOpen(this IDbConnection connection)
    {
        if (connection == null)
            throw new ArgumentNullException("connection");

        if (connection.State != ConnectionState.Open)
        {
            if (connection is IHasOpenedOnce hoo && hoo.OpenedOnce)
                throw new InvalidOperationException("Can't auto open a closed connection " +
                    "that was previously open!");

            connection.Open();
        }

        return connection;
    }

    /// <summary>
    /// Gets the current actual transaction for a connection if any.
    /// Most of the time, a connection will only have one transaction, 
    /// but in .NET it is not possible to know what is that transaction.
    /// Serenity wraps a connection (WrappedConnection) so that running
    /// transaction if any is available to get from the connection object.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The current transaction for a connection</returns>
    public static IDbTransaction GetCurrentActualTransaction(this IDbConnection connection)
    {
        if (connection is IHasCurrentTransaction hct &&
            hct.CurrentTransaction is IHasActualTransaction hat)
            return hat.ActualTransaction;

        return null;
    }

    /// <summary>Sets the default command timeout for given connection. 
    /// Only works with IHasCommandTimeout (WrappedConnection) instances, which are usually
    /// created by SqlConnections.NewXyz methods.</summary>
    /// <param name="connection">The connection.</param>
    /// <param name="timeout">The timeout value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Connection is not a WrappedConnection.</exception>
    public static void SetCommandTimeout(this IDbConnection connection, int? timeout)
    {
        if (connection is IHasCommandTimeout hct)
            hct.CommandTimeout = timeout;
        else
            throw new ArgumentOutOfRangeException(nameof(connection));
    }

    /// <summary>
    /// Gets the dialect for given connection.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The sql dialect.</returns>
    public static ISqlDialect GetDialect(this IDbConnection connection)
    {
        if (connection is not IHasDialect hasDialect)
            return SqlSettings.DefaultDialect;

        return hasDialect.Dialect ?? SqlSettings.DefaultDialect;
    }

    /// <summary>
    /// Gets the logger for a connection if it implements IHasLogger
    /// interface, or null if not.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The logger for connection (used by static SqlHelper methods)</returns>
    public static ILogger GetLogger(this IDbConnection connection)
    {
        return (connection as IHasLogger)?.Logger;
    }
}