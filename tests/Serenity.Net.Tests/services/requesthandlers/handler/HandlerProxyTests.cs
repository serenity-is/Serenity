using System.Threading;

namespace Serenity.Services;

public class HandlerProxyTests
{
    [TableName("ProxyTest")]
    private class TestRow : Row<TestRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
#pragma warning restore CS0649
        }
    }

    private class FakeSave : MockSaveHandler<TestRow>,
        ICreateHandler<TestRow>, IUpdateHandler<TestRow>,
        ICreateHandlerAsync<TestRow>, IUpdateHandlerAsync<TestRow>
    {
        public SaveResponse Create(IUnitOfWork uow, SaveRequest<TestRow> request) => new() { EntityId = 1 };
        public SaveResponse Update(IUnitOfWork uow, SaveRequest<TestRow> request) => new() { EntityId = 2 };
        public Task<SaveResponse> CreateAsync(IUnitOfWork uow, SaveRequest<TestRow> request, CancellationToken cancellationToken = default)
            => Task.FromResult(Create(uow, request));
        public Task<SaveResponse> UpdateAsync(IUnitOfWork uow, SaveRequest<TestRow> request, CancellationToken cancellationToken = default)
            => Task.FromResult(Update(uow, request));
    }

    private class FakeDelete : MockDeleteHandler<TestRow>, IDeleteHandler<TestRow>, IDeleteHandlerAsync<TestRow>
    {
        public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request) => new();
        public Task<DeleteResponse> DeleteAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(Delete(uow, request));
    }

    private class FakeSaveAsync : MockSaveHandlerAsync<TestRow>,
        ICreateHandlerAsync<TestRow>, IUpdateHandlerAsync<TestRow>
    {
        public Task<SaveResponse> CreateAsync(IUnitOfWork uow, SaveRequest<TestRow> request, CancellationToken cancellationToken = default)
            => Task.FromResult(new SaveResponse { EntityId = 1 });
        public Task<SaveResponse> UpdateAsync(IUnitOfWork uow, SaveRequest<TestRow> request, CancellationToken cancellationToken = default)
            => Task.FromResult(new SaveResponse { EntityId = 2 });
    }

    private class FakeDeleteAsync : MockDeleteHandlerAsync<TestRow>, IDeleteHandlerAsync<TestRow>
    {
        public Task<DeleteResponse> DeleteAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new DeleteResponse());
    }

    private class FakeListAsync : MockListHandlerAsync<TestRow>, IListHandlerAsync<TestRow>
    {
        public Task<ListResponse<TestRow>> ListAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new ListResponse<TestRow> { Entities = [] });
    }

    private class FakeList : MockListHandler<TestRow>, IListHandler<TestRow>, IListHandlerAsync<TestRow>
    {
        public ListResponse<TestRow> List(IDbConnection connection, ListRequest request) => new() { Entities = [] };
        public Task<ListResponse<TestRow>> ListAsync(IDbConnection connection, ListRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(List(connection, request));
    }

    private class FakeRetrieve : MockRetrieveHandler<TestRow>,
        IRetrieveRequestProcessor, IRetrieveRequestProcessorAsync,
        IRetrieveHandler<TestRow>, IRetrieveHandlerAsync<TestRow>
    {
        public IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request)
        {
            Connection = connection;
            Request = request;
            return Response;
        }

        public Task<IRetrieveResponse> ProcessAsync(IDbConnection connection, RetrieveRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Process(connection, request));

        public RetrieveResponse<TestRow> Retrieve(IDbConnection connection, RetrieveRequest request)
            => (RetrieveResponse<TestRow>)Process(connection, request);

        public Task<RetrieveResponse<TestRow>> RetrieveAsync(IDbConnection connection, RetrieveRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Retrieve(connection, request));
    }

    private class FakeUndelete : IUndeleteRequestProcessor, IUndeleteRequestProcessorAsync,
        IUndeleteHandler<TestRow>, IUndeleteHandlerAsync<TestRow>
    {
        public IRow Row { get; set; } = new TestRow();
        public UndeleteRequest Request { get; set; } = new();
        public UndeleteResponse Response { get; set; } = new();
        public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
        public IDbConnection? Connection { get; set; }
        public IUnitOfWork? UnitOfWork { get; set; }
        public IRequestContext? Context { get; set; }

        public UndeleteResponse Process(IUnitOfWork uow, UndeleteRequest request)
        {
            UnitOfWork = uow;
            Connection = uow?.Connection;
            Request = request;
            return Response;
        }

        public Task<UndeleteResponse> ProcessAsync(IUnitOfWork uow, UndeleteRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Process(uow, request));

        public UndeleteResponse Undelete(IUnitOfWork uow, UndeleteRequest request) => Process(uow, request);

        public Task<UndeleteResponse> UndeleteAsync(IUnitOfWork uow, UndeleteRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Process(uow, request));
    }

    private static MockHandlerFactory Factory(object handler, Type handlerInterface)
    {
        return new MockHandlerFactory((rowType, iface) =>
        {
            Assert.Equal(typeof(TestRow), rowType);
            Assert.Equal(handlerInterface, iface);
            return handler;
        });
    }

    private static MockUnitOfWork Uow() => new(new MockDbConnection());

    [Fact]
    public void CreateHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void CreateHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeSave(), typeof(ISaveRequestProcessor));
        var proxy = new CreateHandlerProxy<TestRow>(factory);
        Assert.Equal(1, proxy.Create(Uow(), new SaveRequest<TestRow>()).EntityId);
    }

    [Fact]
    public async Task CreateHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeSaveAsync(), typeof(ISaveRequestProcessorAsync));
        var proxy = new CreateHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.CreateAsync(Uow(), new SaveRequest<TestRow>(), TestContext.Current.CancellationToken);
        Assert.Equal(1, response.EntityId);
    }

    [Fact]
    public void UpdateHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new UpdateHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void UpdateHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeSave(), typeof(ISaveRequestProcessor));
        var proxy = new UpdateHandlerProxy<TestRow>(factory);
        Assert.Equal(2, proxy.Update(Uow(), new SaveRequest<TestRow>()).EntityId);
    }

    [Fact]
    public async Task UpdateHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeSaveAsync(), typeof(ISaveRequestProcessorAsync));
        var proxy = new UpdateHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.UpdateAsync(Uow(), new SaveRequest<TestRow>(), TestContext.Current.CancellationToken);
        Assert.Equal(2, response.EntityId);
    }

    [Fact]
    public void DeleteHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new DeleteHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void DeleteHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeDelete(), typeof(IDeleteRequestProcessor));
        var proxy = new DeleteHandlerProxy<TestRow>(factory);
        Assert.NotNull(proxy.Delete(Uow(), new DeleteRequest()));
    }

    [Fact]
    public async Task DeleteHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeDeleteAsync(), typeof(IDeleteRequestProcessorAsync));
        var proxy = new DeleteHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.DeleteAsync(Uow(), new DeleteRequest(), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
    }

    [Fact]
    public void ListHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new ListHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void ListHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeList(), typeof(IListRequestProcessor));
        var proxy = new ListHandlerProxy<TestRow>(factory);
        Assert.NotNull(proxy.List(new MockDbConnection(), new ListRequest()));
    }

    [Fact]
    public async Task ListHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeListAsync(), typeof(IListRequestProcessorAsync));
        var proxy = new ListHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.ListAsync(new MockDbConnection(), new ListRequest(), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
    }

    [Fact]
    public void RetrieveHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new RetrieveHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void RetrieveHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeRetrieve(), typeof(IRetrieveRequestProcessor));
        var proxy = new RetrieveHandlerProxy<TestRow>(factory);
        Assert.NotNull(proxy.Retrieve(new MockDbConnection(), new RetrieveRequest()).Entity);
    }

    [Fact]
    public async Task RetrieveHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeRetrieve(), typeof(IRetrieveRequestProcessorAsync));
        var proxy = new RetrieveHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.RetrieveAsync(new MockDbConnection(), new RetrieveRequest(), TestContext.Current.CancellationToken);
        Assert.NotNull(response.Entity);
    }

    [Fact]
    public void UndeleteHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new UndeleteHandlerProxy<TestRow>(null));
    }

    [Fact]
    public void UndeleteHandlerProxy_Delegates()
    {
        var factory = Factory(new FakeUndelete(), typeof(IUndeleteRequestProcessor));
        var proxy = new UndeleteHandlerProxy<TestRow>(factory);
        Assert.NotNull(proxy.Undelete(Uow(), new UndeleteRequest()));
    }

    [Fact]
    public async Task UndeleteHandlerProxyAsync_Delegates()
    {
        var factory = Factory(new FakeUndelete(), typeof(IUndeleteRequestProcessorAsync));
        var proxy = new UndeleteHandlerProxyAsync<TestRow>(factory);
        var response = await proxy.UndeleteAsync(Uow(), new UndeleteRequest(), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
    }
}

