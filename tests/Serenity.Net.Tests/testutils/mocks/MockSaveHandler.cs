namespace Serenity.TestUtils;

public class MockSaveHandler<TRow>(Action<MockSaveHandler<TRow>> onProcess)
    : ISaveRequestProcessor, IRequestType<SaveRequest<TRow>>
    where TRow : IRow, new()
{
    public IRow Old { get; set; }
    public IRow Row { get; set; } = new TRow();
    public bool IsCreate { get; set; } = true;
    public bool IsUpdate { get; set; } = false;

    public MockSaveHandler()
        : this(_ => { })
    {
    }

    private readonly Action<MockSaveHandler<TRow>> onProcess = onProcess 
        ?? throw new ArgumentNullException(nameof(onProcess));

    public ISaveRequest Request { get; set; } = new SaveRequest<TRow>()
    {
        Entity = new TRow()
    };

    public SaveResponse Response { get; set; } = new()
    {
        EntityId = null
    };

    public SaveRequestType RequestType { get; set; }

    public SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType requestType)
    {
        if (onProcess is null)
            throw new NotImplementedException();

        UnitOfWork = uow;
        Connection = uow?.Connection;
        Request = request;
        RequestType = requestType;

        onProcess(this);

        return Response;
    }

    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }


}