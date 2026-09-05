using System.Threading;

namespace Serenity.TestUtils;

public class MockSaveHandlerAsync<TRow>(Action<MockSaveHandlerAsync<TRow>> onProcessAsync)
    : ISaveRequestProcessorAsync, IRequestType<SaveRequest<TRow>>
    where TRow : IRow, new()
{
    public IRow Old { get; set; }
    public IRow Row { get; set; } = new TRow();
    public bool IsCreate { get; set; } = true;
    public bool IsUpdate { get; set; } = false;

    public MockSaveHandlerAsync()
        : this(_ => { })
    {
    }

    private readonly Action<MockSaveHandlerAsync<TRow>> onProcessAsync = onProcessAsync
        ?? throw new ArgumentNullException(nameof(onProcessAsync));

    public ISaveRequest Request { get; set; } = new SaveRequest<TRow>()
    {
        Entity = new TRow()
    };

    public SaveResponse Response { get; set; } = new()
    {
        EntityId = null
    };

    public SaveRequestType RequestType { get; set; }

    public Task<SaveResponse> ProcessAsync(IUnitOfWork uow, ISaveRequest request, SaveRequestType requestType,
        CancellationToken cancellationToken = default)
    {
        UnitOfWork = uow;
        Connection = uow?.Connection;
        Request = request;
        RequestType = requestType;

        onProcessAsync(this);

        return Task.FromResult(Response);
    }

    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }
}
