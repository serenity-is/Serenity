namespace Serenity.TestUtils;

public class MockDeleteHandler<TRow>(Action<MockDeleteHandler<TRow>> onProcess)
    : IDeleteRequestProcessor, IRequestType<DeleteRequest>
    where TRow : IRow, new()
{
    public MockDeleteHandler()
        : this(_ => { })
    {
    }

    private readonly Action<MockDeleteHandler<TRow>> onProcess = onProcess
        ?? throw new ArgumentNullException(nameof(onProcess));

    public IRow Row { get; set; } = new TRow();
    public DeleteRequest Request { get; set; } = new DeleteRequest();
    public DeleteResponse Response { get; set; } = new DeleteResponse();
    public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
    public IDbConnection Connection { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public IRequestContext Context { get; set; }

    public DeleteResponse Process(IUnitOfWork uow, DeleteRequest request)
    {
        UnitOfWork = uow;
        Connection = uow?.Connection;
        Request = request;

        onProcess(this);

        return Response;
    }
}
