using System.Data.Common;

namespace Serenity.TestUtils;

internal class MockDbTransaction(IDbConnection dbConnection) : DbTransaction
{
    private readonly IDbConnection dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

    public override IsolationLevel IsolationLevel => throw new NotImplementedException();

    protected override DbConnection DbConnection => (DbConnection)dbConnection;

    public override void Commit()
    {
    }

    public override void Rollback()
    {
    }
}
