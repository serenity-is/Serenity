namespace Serenity.Data;

/// <summary>
/// Base arguments shared by intercepted operations.
/// </summary>
public abstract record InterceptOperationArgs
{
    /// <summary>
    /// Gets or initializes the cancellation token for asynchronous execution.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Gets or initializes whether the operation was invoked asynchronously.
    /// </summary>
    public bool IsAsync { get; init; }
}