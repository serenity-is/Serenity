namespace Serenity.Data;

/// <summary>
/// Determines the transaction isolation level and defer start flag used for 
/// a service endpoint action use when creating the UnitOfWork class
/// </summary>
public class TransactionSettingsAttribute : Attribute
{
    private bool? deferStart;

    /// <summary>
    /// Initializes a new instance of the attribute.
    /// </summary>
    public TransactionSettingsAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the attribute.
    /// </summary>
    /// <param name="isolationLevel">The isolation level.</param>
    public TransactionSettingsAttribute(IsolationLevel isolationLevel)
    {
        IsolationLevel = isolationLevel;
    }

    /// <summary>
    /// Gets the isolation level.
    /// </summary>
    /// <value>
    /// The isolation level.
    /// </value>
    public IsolationLevel IsolationLevel { get; private set; } = IsolationLevel.Unspecified;

    /// <summary>
    /// Gets / sets if the transaction start should be deferred if possible
    /// (generally until the connection property of unit of work object is read).
    /// This might have undesired side effects so use with care.
    /// </summary>
    public bool DeferStart
    {
        get => deferStart ?? false;
        set => deferStart = value;
    }

    /// <summary>
    /// Gets if DeferStart property is set
    /// </summary>
    public bool HasDeferStart => deferStart != null;
}