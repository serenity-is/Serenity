namespace Serenity.Data;

/// <summary>
/// Unit of work implementation without an underlying actual transaction.
/// Use with care only to pass a IUnitOfWork instance to some methods
/// that you don't want to actually start a transaction.
/// </summary>
/// <seealso cref="IDisposable" />
/// <seealso cref="IUnitOfWork" />
public class TransactionlessUnitOfWork : IDisposable, IUnitOfWork
{
    private readonly IDbConnection connection;
    private Action onCommit;
    private Action onRollback;
    private bool commitCalled;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="ArgumentNullException">connection</exception>
    public TransactionlessUnitOfWork(IDbConnection connection)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    public IDbConnection Connection => connection;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        onCommit = null;
        try
        {
            onRollback?.Invoke();
        }
        finally
        {
            onRollback = null;
        }
    }

    /// <summary>
    /// Does nothing other than calling onCommit events as there
    /// is no underlying transaction.
    /// </summary>
    public void Commit()
    {
        if (commitCalled)
            throw new InvalidOperationException("Commit is already called!");

        commitCalled = true;

        try
        {
            onCommit?.Invoke();
        }
        finally
        {
            onCommit = null;
        }
    }

    /// <summary>
    /// Occurs when Commit is called as there is no underlying transaction.
    /// </summary>
    public event Action OnCommit
    {
        add { onCommit += value; }
        remove { onCommit -= value; }
    }

    /// <summary>
    /// Occurs when Dispose is called as there is no underlying transaction.
    /// </summary>
    public event Action OnRollback
    {
        add { onRollback += value; }
        remove { onRollback -= value; }
    }
}
