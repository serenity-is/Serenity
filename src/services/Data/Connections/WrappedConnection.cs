using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// Wraps a connection to add current transaction and dialect support.
/// </summary>
/// <seealso cref="DbConnection" />
public class WrappedConnection : DbConnection, IDbConnection, IHasActualConnection, IHasCommandTimeout,
    IHasCurrentTransaction, IHasDialect, IHasLogger, IHasOpenedOnce, IHasConnectionStateChange
{
    private readonly IDbConnection actualConnection;
    private ISqlDialect dialect;
    private readonly ILogger logger;
    private bool openedOnce;
    private WrappedTransaction currentTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedConnection"/> class.
    /// </summary>
    /// <param name="connection">The actual connection.</param>
    /// <param name="dialect">The dialect.</param>
    /// <param name="logger">Optional logger for this connection (generally to be used by static SqlHelper methods)</param>
    public WrappedConnection(IDbConnection connection, ISqlDialect dialect, ILogger logger = null)
    {
        actualConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.dialect = dialect;
        this.logger = logger;

        if (actualConnection is DbConnection dbConnection)
            dbConnection.StateChange += (s, e) => OnStateChange(e);
        else if (actualConnection is IHasConnectionStateChange hasStateChange)
            hasStateChange.StateChange += (s, e) => OnStateChange(e);
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
    /// <param name="isolationLevel">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
    /// <returns>
    /// An object representing the new transaction.
    /// </returns>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        var actualTransaction = isolationLevel == IsolationLevel.Unspecified
            ? actualConnection.BeginTransaction()
            : actualConnection.BeginTransaction(isolationLevel);
        currentTransaction = new WrappedTransaction(this, actualTransaction);
        return currentTransaction;
    }

    /// <summary>
    /// Begins a database transaction asynchronously with the specified <see cref="T:System.Data.IsolationLevel"></see> value.
    /// </summary>
    /// <param name="isolationLevel">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A value task that represents the asynchronous operation. The task result contains the new transaction.</returns>
    protected override async ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
    {
        IDbTransaction actualTransaction;
        if (actualConnection is DbConnection dbConnection)
            actualTransaction = await dbConnection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        else
            actualTransaction = isolationLevel == IsolationLevel.Unspecified
                ? actualConnection.BeginTransaction()
                : actualConnection.BeginTransaction(isolationLevel);

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
    public override void ChangeDatabase(string databaseName)
    {
        actualConnection.ChangeDatabase(databaseName);
    }

    /// <summary>
    /// Closes the connection to the database.
    /// </summary>
    public override void Close()
    {
        actualConnection.Close();
    }

    /// <summary>
    /// Closes the connection to the database asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override Task CloseAsync()
    {
        if (actualConnection is DbConnection dbConnection)
            return dbConnection.CloseAsync();
        return Task.Run(() => actualConnection.Close());
    }

    /// <summary>
    /// Gets or sets the string used to open a database.
    /// </summary>
    public override string ConnectionString
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
    public override int ConnectionTimeout => actualConnection.ConnectionTimeout;

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
    IDbCommand IDbConnection.CreateCommand()
    {
        var command = actualConnection.CreateCommand();
        try
        {
            SetupCommand(command);
            return command;
        }
        catch
        {
            command.Dispose();
            throw;
        }
    }

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
    protected override DbCommand CreateDbCommand()
    {
        var command = actualConnection.CreateCommand();
        try
        {
            if (command is not DbCommand dbCommand)
                throw new NotSupportedException("CreateCommand is not supported for connections that do not return a DbCommand from CreateCommand()!");

            SetupCommand(dbCommand);
            return dbCommand;
        }
        catch
        {
            command.Dispose();
            throw;
        }
    }

    private void SetupCommand(IDbCommand command)
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

    /// <summary>
    /// Gets the name of the current database or the database to be used after a connection is opened.
    /// </summary>
    public override string Database => actualConnection.Database;

    /// <summary>
    /// Gets the name of the database server to which to connect.
    /// </summary>
    public override string DataSource =>
        actualConnection is DbConnection dbConnection ? dbConnection.DataSource
        : throw new NotSupportedException("DataSource is not supported for connections that do not derive from DbConnection!");

    /// <summary>
    /// Gets the version of the database server.
    /// </summary>
    public override string ServerVersion =>
        actualConnection is DbConnection dbConnection ? dbConnection.ServerVersion
        : throw new NotSupportedException("ServerVersion is not supported for connections that do not derive from DbConnection!");

    /// <summary>
    /// Gets the associated provider factory for the connection, or <c>null</c> if the actual
    /// connection is not a <see cref="DbConnection"/>.
    /// </summary>
    protected override DbProviderFactory DbProviderFactory =>
        actualConnection is DbConnection dbConnection ? DbProviderFactories.GetFactory(dbConnection) : null;

    /// <summary>
    /// Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.
    /// </summary>
    public override void Open()
    {
        actualConnection.Open();
        openedOnce = true;
    }

    /// <summary>
    /// Opens a database connection asynchronously with the settings specified by the ConnectionString
    /// property of the provider-specific Connection object.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        if (actualConnection is DbConnection dbConnection)
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        else
            actualConnection.Open();

        openedOnce = true;
    }

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    public override ConnectionState State => actualConnection.State;

    /// <summary>
    /// Gets the logger instance for this connection, if any.
    /// </summary>
    public ILogger Logger => logger;

    /// <summary>
    /// Disposes the actual connection.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            actualConnection.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Disposes the actual connection asynchronously.
    /// </summary>
    /// <returns>A value task that represents the asynchronous operation.</returns>
    public override ValueTask DisposeAsync()
    {
        if (actualConnection is DbConnection dbConnection)
            return dbConnection.DisposeAsync();
        actualConnection.Dispose();
        return ValueTask.CompletedTask;
    }
}