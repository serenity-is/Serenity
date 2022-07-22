namespace Serenity.Tests;

public class MockUnitOfWork : IUnitOfWork
{
    public IDbConnection Connection { get; }
    public event Action OnCommit;
    public event Action OnRollback;
    
    public MockUnitOfWork(IDbConnection connection = null,
        Action onCommit = null,
        Action onRollback = null)
    {
        Connection = connection;
        OnCommit = onCommit; 
        OnRollback = onRollback;
    }
    
    public IEnumerable<Delegate> OnCommitInvocationList => OnCommit?.GetInvocationList() ?? Array.Empty<Delegate>();
    public IEnumerable<Delegate> OnRollbackInvocationList => OnRollback?.GetInvocationList() ?? Array.Empty<Delegate>();
    
    public void Commit()
    {
        OnCommit?.Invoke();
    }
    
    public void Rollback()
    {
        OnRollback?.Invoke();
    }
}