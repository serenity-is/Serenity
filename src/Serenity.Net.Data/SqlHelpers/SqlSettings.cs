using System.Threading;

namespace Serenity.Data;

/// <summary>
/// Global SQL settings
/// </summary>
public static class SqlSettings
{
    private static ISqlDialect defaultDialect;
    private static readonly AsyncLocal<ISqlDialect> localDialect;

    static SqlSettings()
    {
        defaultDialect = new SqlServer2012Dialect();
        localDialect = new AsyncLocal<ISqlDialect>();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically quote identifiers.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should automatically quote identifiers; otherwise, <c>false</c>.
    /// </value>
    public static bool AutoQuotedIdentifiers { get; set; }

    /// <summary>
    /// Gets or sets the default command timeout.
    /// </summary>
    /// <value>
    /// The default command timeout.
    /// </value>
    public static int? DefaultCommandTimeout { get; set; }
   
    /// <summary>
    /// The default dialect, returns the local dialect if any set through
    /// SetLocal, the default dialect otherwise.
    /// This should be only set on application start.
    /// Local dialect should be used for unit tests.
    /// </summary>
    public static ISqlDialect DefaultDialect
    {
        get => localDialect.Value ?? defaultDialect; 
        set => defaultDialect = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Sets local dialect for current thread and async context. 
    /// Useful for background tasks, async methods, and testing to 
    /// set dialect locally and for auto spawned threads.
    /// </summary>
    /// <param name="dialect">Dialect. Can be null.</param>
    /// <returns>Old local dialect if any.</returns>
    public static ISqlDialect SetLocalDialect(ISqlDialect dialect)
    {
        var old = localDialect.Value;
        localDialect.Value = dialect;
        return old;
    }
}