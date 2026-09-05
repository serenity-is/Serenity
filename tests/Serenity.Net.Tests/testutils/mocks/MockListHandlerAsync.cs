using System.Threading;

namespace Serenity.TestUtils;

public class MockListHandlerAsync<TRow>(Action<MockListHandlerAsync<TRow>> onProcessAsync)
    : IListRequestProcessorAsync, IRequestType<ListRequest>
    where TRow : IRow, new()
{
    private readonly Action<MockListHandlerAsync<TRow>> onProcessAsync = onProcessAsync
        ?? throw new ArgumentNullException(nameof(onProcessAsync));

    public MockListHandlerAsync()
        : this(_ => { })
    {
    }

    public TRow Row { get; set; } = new TRow();

    public ListRequest Request { get; set; } = new ListRequest();

    public ListResponse<TRow> Response { get; set; } = new ListResponse<TRow>()
    {
        Entities = []
    };

    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IRequestContext Context { get; set; }

    IListResponse IListRequestHandler.Response => Response;
    IRow IListRequestHandler.Row => Row;

    public virtual bool AllowSelectField(Field field) => true;
    public void IgnoreEqualityFilter(string field) { }
    public virtual bool ShouldSelectField(Field field) => true;

    public Task<IListResponse> ProcessAsync(IDbConnection connection, ListRequest request,
        CancellationToken cancellationToken = default)
    {
        Connection = connection;
        Request = request;

        onProcessAsync(this);

        return Task.FromResult((IListResponse)Response);
    }
}
