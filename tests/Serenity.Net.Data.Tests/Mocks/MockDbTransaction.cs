namespace Serenity.Tests;

internal class MockDbTransaction : IDbTransaction
{
    private readonly IDbConnection dbConnection;

    public MockDbTransaction(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
    }

    public IDbConnection Connection => dbConnection;

    public IsolationLevel IsolationLevel => throw new NotImplementedException();


    public void Commit()
    {
    }

    public void Dispose()
    {
    }

    public void Rollback()
    {
    }
}
