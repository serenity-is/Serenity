namespace Serenity.Tests;

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

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
    }

    public void Open()
    {
        isOpen = true;
    }
}