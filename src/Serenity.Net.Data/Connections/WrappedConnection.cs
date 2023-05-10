using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// Wraps a connection to add current transaction and dialect support.
/// </summary>
/// <seealso cref="IDbConnection" />
public class WrappedConnection : IDbConnection, IHasActualConnection, IHasCommandTimeout, 
    IHasCurrentTransaction, IHasDialect, IHasLogger, IHasOpenedOnce, IHasConnectionStateChange
{
    private readonly IDbConnection actualConnection;
    private bool openedOnce;
    private WrappedTransaction currentTransaction;
    private ISqlDialect dialect;
    private readonly ILogger logger;

    /// <summary>
    /// Implements state change event by proxying it to the actual connection
    /// </summary>
    public event StateChangeEventHandler StateChange 
    { 
        add 
        {
            if (actualConnection is DbConnection dbConnection)
                dbConnection.StateChange += value;
            else if (actualConnection is IHasConnectionStateChange hasStateChange)
                hasStateChange.StateChange += value;
            else
                throw new NotImplementedException();
        } 
        remove 
        {
            if (actualConnection is DbConnection dbConnection)
                dbConnection.StateChange -= value;
            else if (actualConnection is IHasConnectionStateChange hasStateChange)
                hasStateChange.StateChange -= value;
            else
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedConnection"/> class.
    /// </summary>
    /// <param name="actualConnection">The actual connection.</param>
    /// <param name="dialect">The dialect.</param>
    /// <param name="logger">Optional logger for this connection (generally to be used by static SqlHelper methods)</param>
    public WrappedConnection(IDbConnection actualConnection, ISqlDialect dialect, ILogger logger = null)
    {
        this.actualConnection = actualConnection;
        this.dialect = dialect;
        this.logger = logger;
    }

    /// <summary>
    /// Gets a value indicating whether the connection was opened once.
    /// </summary>
    /// <value>
    ///   <c>true</c> if opened once; otherwise, <c>false</c>.
    /// </value>
    public bool OpenedOnce => openedOnce;

    /// <summary>
    /// Gets the actual connection instance.
    /// </summary>
    /// <value>
    /// The actual connection.
    /// </value>
    public IDbConnection ActualConnection => actualConnection;

    /// <summary>
    /// Gets or sets the SQL dialect.
    /// </summary>
    /// <value>
    /// The SQL dialect.
    /// </value>
    public ISqlDialect Dialect
    {
        get { return dialect; }
        set { dialect = value; }
    }

    /// <summary>
    /// Gets the current transaction.
    /// </summary>
    /// <value>
    /// The current transaction.
    /// </value>
    public IDbTransaction CurrentTransaction => currentTransaction;

    /// <summary>
    /// Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"></see> value.
    /// </summary>
    /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
    /// <returns>
    /// An object representing the new transaction.
    /// </returns>
    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        var actualTransaction = actualConnection.BeginTransaction(il);
        currentTransaction = new WrappedTransaction(this, actualTransaction);
        return currentTransaction;
    }

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>
    /// An object representing the new transaction.
    /// </returns>
    public IDbTransaction BeginTransaction()
    {
        var actualTransaction = actualConnection.BeginTransaction();
        currentTransaction = new WrappedTransaction(this, actualTransaction);
        return currentTransaction;
    }

    internal void Release(WrappedTransaction transaction)
    {
        if (currentTransaction == transaction)
        {
            currentTransaction = null;
        }
    }

    /// <summary>
    /// Changes the current database for an open Connection object.
    /// </summary>
    /// <param name="databaseName">The name of the database to use in place of the current database.</param>
    public void ChangeDatabase(string databaseName)
    {
        actualConnection.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public void Close()
    {
        actualConnection.Close();
    }

    /// <summary>
    /// Gets or sets the string used to open a database.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return actualConnection.ConnectionString;
        }
        set
        {
            actualConnection.ConnectionString = value;
        }
    }

    /// <summary>
    /// Gets or sets default command timeout.
    /// </summary>
    /// <value>
    /// Default command timeout.
    /// </value>
    public int? CommandTimeout { get; set; }

    /// <summary>
    /// Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.
    /// </summary>
    public int ConnectionTimeout => actualConnection.ConnectionTimeout;

    /// <summary>
    /// Creates and returns a Command object associated with the connection.
    /// </summary>
    /// <returns>
    /// A Command object associated with the connection.
    /// </returns>
    /// <exception cref="System.Exception">
    /// Active transaction for connection is in invalid state! " + 
    ///                         "Connection was probably closed unexpectedly!
    /// or
    /// Can't set transaction for command! " +
    ///                         "Connection was probably closed unexpectedly!
    /// </exception>
    public IDbCommand CreateCommand()
    {
        var command = actualConnection.CreateCommand();
        try
        {
            if (CommandTimeout.HasValue)
                command.CommandTimeout = CommandTimeout.Value;
            else if (SqlSettings.DefaultCommandTimeout.HasValue)
                command.CommandTimeout = SqlSettings.DefaultCommandTimeout.Value;

            var transaction = currentTransaction?.ActualTransaction;
            if (transaction != null && transaction.Connection == null)
                throw new System.Exception("Active transaction for connection is in invalid state! " +
                    "Connection was probably closed unexpectedly!");

            command.Transaction = transaction;

            if (transaction != null && command.Transaction == null)
                throw new System.Exception("Can't set transaction for command! " +
                    "Connection was probably closed unexpectedly!");
        }
        catch
        {
            command.Dispose();
            throw;
        }

        return command;
    }

    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened.
    /// </summary>
    public string Database => actualConnection.Database;

    /// <summary>
    /// Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.
    /// </summary>
    public void Open()
    {
        actualConnection.Open();
        openedOnce = true;
    }

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    public ConnectionState State => actualConnection.State;

    /// <summary>
    /// Gets the logger instance for this connection if any
    /// </summary>
    public ILogger Logger => logger;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        actualConnection.Dispose();
    }
}