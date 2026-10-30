using Microsoft.Data.Sqlite;

namespace Serenity.Testing;

public class MemoryDbInstance : IDisposable
{
    private readonly string connectionString;
    private readonly WrappedConnection connection;
    private bool disposed;
    private readonly ISqlDialect localDialect;
    private readonly HashSet<string> schemas;
    private readonly string memDatabaseId;
    private SqliteConnection snapshot;

    public MemoryDbInstance(string prefix = "")
    {
        memDatabaseId = prefix + Guid.NewGuid().ToString("N");

        connectionString = "Data Source=" +
            memDatabaseId +
            ";Mode=Memory;Cache=Shared";

        localDialect = SqlSettings.SetLocalDialect(SqliteDialect.Instance);
        connection = new WrappedConnection(new SqliteConnection(connectionString),
            SqliteDialect.Instance);
        connection.Open();
        schemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public void CreateSnapshot()
    {
        var targetCS = "Data Source=" + memDatabaseId + "_snapshot" +
            ";Mode=Memory;Cache=Shared";
        var source = (SqliteConnection)((WrappedConnection)connection).ActualConnection;
        if (snapshot == null)
        {
            snapshot = new SqliteConnection(targetCS);
            try
            {
                snapshot.Open();
                source.BackupDatabase(snapshot);
            }
            catch
            {
                snapshot = null;
                throw;
            }
        }
        else
            source.BackupDatabase(snapshot);
    }

    public void RestoreSnapshot()
    {
        if (snapshot == null)
            return;

        var target = (SqliteConnection)connection.ActualConnection;
        SqliteConnection.ClearAllPools();
        for (var i = 0; i < 10; i++)
            try
            {
                snapshot.BackupDatabase(target);
                break;
            }
            catch (SqliteException)
            {
                Thread.Sleep(5);
            }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                SqlSettings.SetLocalDialect(localDialect);
                connection?.Dispose();
            }

            disposed = true;
        }
    }

    public void EnsureSchema(string schema)
    {
        if (schemas.Contains(schema))
            return;

        AttachSchema(Connection, schema);

        schemas.Add(schema);
    }

    private void AttachSchema(IDbConnection connection, string schema)
    {
        SqlHelper.ExecuteNonQuery(connection, "ATTACH DATABASE 'file:" +
            memDatabaseId + "_" + schema + "?mode=memory&cache=shared' AS " + schema);
    }

    public IReadOnlySet<string> Schemas => schemas;
    public IDbConnection Connection => connection;
    public ISet<string> FallbackFor { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void CreateFunction<T1, TResult>(string name, Func<T1, TResult> function,
        bool isDeterministic = false)
    {
        var connection = ((SqliteConnection)((IHasActualConnection)Connection).ActualConnection);
        connection.CreateFunction(name, function, isDeterministic);
        OnConnectionOpen += (cnn) =>
        {
            cnn.CreateFunction(name, function, isDeterministic);
        };
    }

    public Action<SqliteConnection> OnConnectionOpen;

    internal string ConnectionString => connectionString;

    internal IDbConnection AttachSchemasOnOpen(IDbConnection connection)
    {
        var dbConnection = connection as SqliteConnection ??
            (connection as IHasActualConnection)?.ActualConnection as SqliteConnection;
        dbConnection?.StateChange += (s, e) =>
            {
                if (e.OriginalState != ConnectionState.Open &&
                    e.CurrentState == ConnectionState.Open)
                {
                    foreach (var schema in schemas)
                        AttachSchema(dbConnection, schema);

                    OnConnectionOpen?.Invoke(dbConnection);
                }
            };

        return connection;
    }
}