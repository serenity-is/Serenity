namespace Serenity.Tests;

public class MockRetrieveHandler<TRow> : IRetrieveRequestHandler
    where TRow: IRow, new()
{
    public TRow Row { get; set; } = new TRow();

    public RetrieveRequest Request { get; set; } = new RetrieveRequest();

    public RetrieveResponse<TRow> Response { get; set; } = new RetrieveResponse<TRow>()
    {
        Entity = new TRow()
    };

    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();

    public IDbConnection Connection { get; set; }

    public IRequestContext Context { get; set; }

    IRetrieveResponse IRetrieveRequestHandler.Response => Response;
    IRow IRetrieveRequestHandler.Row => Row;

    public virtual bool AllowSelectField(Field field)
    {
        return true;
    }

    public virtual bool ShouldSelectField(Field field)
    {
        return true;
    }
}