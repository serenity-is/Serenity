namespace Serenity.Tests;

public class MockSaveHandler<TRow> : ISaveRequestHandler
    where TRow: IRow, new()
{
    public IRow Old { get; set; }
    public IRow Row { get; set; } = new TRow();
    public bool IsCreate { get; set; } = true;
    public bool IsUpdate { get; set; } = false;

    public ISaveRequest Request { get; set; } = new SaveRequest<TRow>()
    {
        Entity = new TRow()
    };
    
    public SaveResponse Response { get; set; }  = new()
    {
        EntityId = null
    };
    
    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }
    
    
}