namespace Serenity.TestUtils;

public class NullDbConnection : IDbConnection
{
    private bool isOpen;

    public string ConnectionString { get; set; }
    public int ConnectionTimeout => 0;
    public string Database => "!Database!";
    public ConnectionState State => isOpen ? ConnectionState.Open : ConnectionState.Closed;
    public IDbTransaction BeginTransaction() => throw new System.NotImplementedException();
    public IDbTransaction BeginTransaction(IsolationLevel il) => throw new System.NotImplementedException();
    public void ChangeDatabase(string databaseName) => throw new System.NotImplementedException();
    public void Close() { isOpen = false; }
    public IDbCommand CreateCommand() => throw new System.NotImplementedException();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Open()
    {
        isOpen = true;
    }
}