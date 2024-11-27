using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// Unit of work implementation.
/// </summary>
/// <seealso cref="IDisposable" />
/// <seealso cref="IUnitOfWork" />
public class UnitOfWork : IDisposable, IUnitOfWork
{
    private readonly IDbConnection connection;
    private IDbTransaction transaction;
    private Action onCommit;
    private Action onRollback;
    private readonly IsolationLevel isolationLevel;
    private bool initialized;
    private bool commited;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="ArgumentNullException">connection</exception>
    public UnitOfWork(IDbConnection connection)
        : this(connection, IsolationLevel.Unspecified, deferStart: false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <exception cref="ArgumentNullException">connection</exception>
    /// <param name="deferStart">Defers starting of the transaction until the connection is opened 
    /// if it has a statechange event, or the first moment connection property is read. 
    /// If the connection is already open this flag has no effect.
    /// If the passed connection does not have a statechange event and is accessed somewhere else 
    /// (e.g. other than via the UnitOfWork.Connection property),  it may cause consistency 
    /// issues so ensure it is not accessed via other means.</param>
    /// <exception cref="ArgumentNullException">connection</exception>
    public UnitOfWork(IDbConnection connection, bool deferStart)
        : this(connection, IsolationLevel.Unspecified, deferStart)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class
    /// with the specified System.Data.IsolationLevel value.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
    /// <param name="deferStart">Defers starting of the transaction until the connection is opened 
    /// if it has a statechange event, or the first moment connection property is read. 
    /// If the connection is already open this flag has no effect.
    /// If the passed connection does not have a statechange event and is accessed somewhere else 
    /// (e.g. other than via the UnitOfWork.Connection property),  it may cause consistency 
    /// issues so ensure it is not accessed via other means.</param>
    /// <exception cref="ArgumentNullException">connection</exception>
    public UnitOfWork(IDbConnection connection, IsolationLevel il, bool deferStart = false)
    {
        this.connection = connection ?? throw new ArgumentNullException("connection");
        isolationLevel = il;
        if (!deferStart || connection.State == ConnectionState.Open)
            Initialize();
        else
        {
            if (connection is IHasConnectionStateChange hasStateChange)
                hasStateChange.StateChange += StateChangeHandler;
            else if (connection is DbConnection dbConnection)
                dbConnection.StateChange += StateChangeHandler;
        }
    }

    private void UnbindStateChange()
    {
        if (connection is IHasConnectionStateChange hasStateChange)
            hasStateChange.StateChange -= StateChangeHandler;
        else if (connection is DbConnection dbConnection)
            dbConnection.StateChange -= StateChangeHandler;
    }

    private void StateChangeHandler(object sender, StateChangeEventArgs e)
    {
        if (e.CurrentState == ConnectionState.Open &&
            e.OriginalState != ConnectionState.Open)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        if (initialized || disposed)
            return;

        initialized = true;
        UnbindStateChange();

        connection.EnsureOpen();
        transaction = isolationLevel == IsolationLevel.Unspecified ? connection.BeginTransaction()
            : connection.BeginTransaction(isolationLevel);
    }

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    public IDbConnection Connection
    {
        get
        {
            if (connection is not IHasConnectionStateChange &&
                connection is not DbConnection)
                Initialize();

            return connection;
        }
    }

    /// <summary>
    /// Rollbacks the transaction if any and calls onRollback event.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;

        UnbindStateChange();

        onCommit = null;
        try
        {
            try
            {
                transaction?.Dispose();
            }
            finally
            { 
                onRollback?.Invoke();
            }
        }
        finally
        {
            transaction = null;
            onRollback = null;
            disposed = true;
        }
    }

    /// <summary>
    /// Commits this transaction.
    /// </summary>
    /// <exception cref="ArgumentNullException">transaction</exception>
    public void Commit()
    {
        if (commited)
            throw new InvalidOperationException("Transaction is already committed!");
         
        transaction?.Commit();
        commited = true;

        onRollback = null;
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
    /// Returns true if the transaction is tried to be started at least once.
    /// This always returns true if deferStart is not true.
    /// </summary>
    public bool Initialized => initialized;

    /// <summary>
    /// Occurs when transaction is committed.
    /// </summary>
    public event Action OnCommit
    {
        add { onCommit += value; }
        remove { onCommit -= value; }
    }

    /// <summary>
    /// Occurs when transaction is rolled back.
    /// </summary>
    public event Action OnRollback
    {
        add { onRollback += value; }
        remove { onRollback -= value; }
    }
}
