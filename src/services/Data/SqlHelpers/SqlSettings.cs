namespace Serenity.Data;

/// <summary>
/// Global SQL settings.
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
    /// This is used as a fallback if the dialect and <see cref="SqlSettings.DefaultDialect"/> do
    /// not provide a value. Default is true.
    /// </summary>
    /// <value>
    ///   <c>true</c> if identifiers should be automatically quoted; otherwise, <c>false</c>.
    /// </value>
    public static bool AutoQuotedIdentifiers { get; set; } = true;

    /// <summary>
    /// Gets or sets the default command timeout.
    /// </summary>
    /// <value>
    /// The default command timeout.
    /// </value>
    public static int? DefaultCommandTimeout { get; set; }
   
    /// <summary>
    /// Gets or sets the default dialect. Returns the local dialect if any is set through
    /// <see cref="SetLocalDialect"/>, otherwise the default dialect.
    /// This should only be set on application start.
    /// The local dialect should be used for unit tests.
    /// </summary>
    public static ISqlDialect DefaultDialect
    {
        get => localDialect.Value ?? defaultDialect; 
        set => defaultDialect = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Sets the local dialect for the current thread and async context.
    /// Useful for background tasks, async methods, and testing to
    /// set the dialect locally and for auto spawned threads.
    /// </summary>
    /// <param name="dialect">The dialect. Can be null.</param>
    /// <returns>The old local dialect, if any.</returns>
    public static ISqlDialect SetLocalDialect(ISqlDialect dialect)
    {
        var old = localDialect.Value;
        localDialect.Value = dialect;
        return old;
    }
}