namespace Serenity.Tests;

public class MockListHandler<TRow> : IListRequestProcessor, IRequestType<ListRequest>
    where TRow: IRow, new()
{
    private readonly Action<MockListHandler<TRow>> onProcess;

    public MockListHandler()
    {
    }

    public MockListHandler(Action<MockListHandler<TRow>> onProcess)
    {
        this.onProcess = onProcess ?? throw new ArgumentNullException(nameof(onProcess));
    }

    public TRow Row { get; set; } = new TRow();

    public ListRequest Request { get; set; } = new ListRequest();

    public ListResponse<TRow> Response { get; set; } = new ListResponse<TRow>()
    {
        Entities = new List<TRow>()
    };

    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();

    public IDbConnection Connection { get; set; }

    public IRequestContext Context { get; set; }

    IListResponse IListRequestHandler.Response => Response;
    IRow IListRequestHandler.Row => Row;

    public virtual bool AllowSelectField(Field field)
    {
        return true;
    }

    public void IgnoreEqualityFilter(string field)
    {
    }

    public IListResponse Process(IDbConnection connection, ListRequest request)
    {
        if (onProcess is null)
            throw new NotImplementedException();

        Connection = connection;
        Request = request;

        onProcess(this);

        return Response;
    }

    public virtual bool ShouldSelectField(Field field)
    {
        return true;
    }
}