namespace Serenity.Tests;

internal class MockDbTransaction(IDbConnection dbConnection) : IDbTransaction
{
    private readonly IDbConnection dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

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
