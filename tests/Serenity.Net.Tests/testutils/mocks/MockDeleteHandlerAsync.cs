using System.Threading;

namespace Serenity.TestUtils;

public class MockDeleteHandlerAsync<TRow>(Action<MockDeleteHandlerAsync<TRow>> onProcessAsync)
    : IDeleteRequestProcessorAsync, IRequestType<DeleteRequest>
    where TRow : IRow, new()
{
    private readonly Action<MockDeleteHandlerAsync<TRow>> onProcessAsync = onProcessAsync
        ?? throw new ArgumentNullException(nameof(onProcessAsync));

    public MockDeleteHandlerAsync()
        : this(_ => { })
    {
    }

    public IRow Row { get; set; } = new TRow();
    public DeleteRequest Request { get; set; } = new DeleteRequest();
    public DeleteResponse Response { get; set; } = new DeleteResponse();
    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }

    public Task<DeleteResponse> ProcessAsync(IUnitOfWork uow, DeleteRequest request,
        CancellationToken cancellationToken = default)
    {
        UnitOfWork = uow;
        Connection = uow?.Connection;
        Request = request;

        onProcessAsync(this);

        return Task.FromResult(Response);
    }
}
