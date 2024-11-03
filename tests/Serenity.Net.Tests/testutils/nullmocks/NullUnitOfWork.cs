namespace Serenity.Tests;

public class NullUnitOfWork(IDbConnection connection = null) : IUnitOfWork
{
    private readonly IDbConnection connection = connection ?? new NullDbConnection();

    public IDbConnection Connection => connection;

    public event Action OnCommit
    {
        add { }
        remove { }
    }
    public event Action OnRollback
    {
        add { }
        remove { }
    }
}