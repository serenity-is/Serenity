namespace Serenity.Tests;

public class NullUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection connection;

    public NullUnitOfWork(IDbConnection connection = null)
    {
        this.connection = connection ?? new NullDbConnection();
    }

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