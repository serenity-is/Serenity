namespace Serenity.Tests;

public class MockDeleteHandler<TRow> : IDeleteRequestHandler
    where TRow: IRow, new()
{
    public IRow Row { get; set; } = new TRow();

    public DeleteRequest Request { get; } = new DeleteRequest();
    public DeleteResponse Response { get; } = new DeleteResponse();
    
    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }
    
    
}