using System.Threading;

namespace Serenity.Services;

public class RequestProcessorWrapperTests
{
    [TableName("WrapTest")]
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

    private class FakeRetrieveProcessor<TRow> : MockRetrieveHandler<TRow>, IRetrieveRequestProcessor, IRetrieveRequestProcessorAsync
        where TRow : IRow, new()
    {
        public IRetrieveResponse Process(IDbConnection connection, RetrieveRequest request)
        {
            Connection = connection;
            Request = request;
            return Response;
        }

        public Task<IRetrieveResponse> ProcessAsync(IDbConnection connection, RetrieveRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Process(connection, request));
        }
    }

    private class FakeUndeleteProcessor<TRow> : IUndeleteRequestProcessor, IUndeleteRequestProcessorAsync
        where TRow : class, IRow, IIdRow, new()
    {
        public IRow Row { get; set; } = new TRow();
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
        {
            return Task.FromResult(Process(uow, request));
        }
    }

    private static MockUnitOfWork Uow()
    {
        return new MockUnitOfWork(new MockDbConnection());
    }

    [Fact]
    public void Save_Wrappers_Throw_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncToAsyncSaveRequestProcessorWrapper<TestRow>(null));
        Assert.Throws<ArgumentNullException>(() => new AsyncToSyncSaveRequestProcessorWrapper<TestRow>(null));
    }

    [Fact]
    public async Task SyncToAsyncSave_Wraps()
    {
        var mock = new MockSaveHandler<TestRow>();
        var wrapper = new SyncToAsyncSaveRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new SaveRequest<TestRow> { Entity = new TestRow() };

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(await wrapper.CreateAsync(uow, request, TestContext.Current.CancellationToken));
        Assert.Equal(SaveRequestType.Create, mock.RequestType);
        Assert.NotNull(await wrapper.UpdateAsync(uow, request, TestContext.Current.CancellationToken));
        Assert.Equal(SaveRequestType.Update, mock.RequestType);
        Assert.NotNull(await wrapper.ProcessAsync(uow, request, SaveRequestType.Create, TestContext.Current.CancellationToken));

        var handler = (ISaveRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
        Assert.Same(mock.Connection, handler.Connection);
        Assert.Same(mock.UnitOfWork, handler.UnitOfWork);
        Assert.Equal(mock.IsCreate, handler.IsCreate);
        Assert.Equal(mock.IsUpdate, handler.IsUpdate);
        Assert.Null(handler.Old);
        Assert.NotNull(handler.Request);
        Assert.Null(handler.Context);
    }

    [Fact]
    public void AsyncToSyncSave_Wraps()
    {
        var mock = new MockSaveHandlerAsync<TestRow>();
        var wrapper = new AsyncToSyncSaveRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new SaveRequest<TestRow> { Entity = new TestRow() };

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(wrapper.Create(uow, request));
        Assert.Equal(SaveRequestType.Create, mock.RequestType);
        Assert.NotNull(wrapper.Update(uow, request));
        Assert.Equal(SaveRequestType.Update, mock.RequestType);
        Assert.NotNull(wrapper.Process(uow, request, SaveRequestType.Update));

        var handler = (ISaveRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
    }

    [Fact]
    public void List_Wrappers_Throw_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncToAsyncListRequestProcessorWrapper<TestRow>(null));
        Assert.Throws<ArgumentNullException>(() => new AsyncToSyncListRequestProcessorWrapper<TestRow>(null));
    }

    [Fact]
    public async Task SyncToAsyncList_Wraps()
    {
        var mock = new MockListHandler<TestRow>();
        var wrapper = new SyncToAsyncListRequestProcessorWrapper<TestRow>(mock);
        var connection = new MockDbConnection();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(await wrapper.ProcessAsync(connection, new ListRequest(), TestContext.Current.CancellationToken));
        Assert.NotNull(await wrapper.ListAsync(connection, new ListRequest(), TestContext.Current.CancellationToken));

        var handler = (IListRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
        Assert.True(handler.AllowSelectField(null));
        Assert.True(handler.ShouldSelectField(null));
        handler.IgnoreEqualityFilter("X");
        Assert.Same(connection, handler.Connection);
        Assert.NotNull(handler.Request);
        Assert.Null(handler.Context);
    }

    [Fact]
    public void AsyncToSyncList_Wraps()
    {
        var mock = new MockListHandlerAsync<TestRow>();
        var wrapper = new AsyncToSyncListRequestProcessorWrapper<TestRow>(mock);
        var connection = new MockDbConnection();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(wrapper.Process(connection, new ListRequest()));
        Assert.NotNull(wrapper.List(connection, new ListRequest()));

        var handler = (IListRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.True(handler.AllowSelectField(null));
        Assert.True(handler.ShouldSelectField(null));
        handler.IgnoreEqualityFilter("X");
    }

    [Fact]
    public void Delete_Wrappers_Throw_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncToAsyncDeleteRequestProcessorWrapper<TestRow>(null));
        Assert.Throws<ArgumentNullException>(() => new AsyncToSyncDeleteRequestProcessorWrapper<TestRow>(null));
    }

    [Fact]
    public async Task SyncToAsyncDelete_Wraps()
    {
        var mock = new MockDeleteHandler<TestRow>();
        var wrapper = new SyncToAsyncDeleteRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new DeleteRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(await wrapper.ProcessAsync(uow, request, TestContext.Current.CancellationToken));
        Assert.NotNull(await wrapper.DeleteAsync(uow, request, TestContext.Current.CancellationToken));

        var handler = (IDeleteRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
        Assert.Same(mock.UnitOfWork, handler.UnitOfWork);
        Assert.NotNull(handler.Request);
        Assert.Null(handler.Context);
    }

    [Fact]
    public void AsyncToSyncDelete_Wraps()
    {
        var mock = new MockDeleteHandlerAsync<TestRow>();
        var wrapper = new AsyncToSyncDeleteRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new DeleteRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(wrapper.Process(uow, request));
        Assert.NotNull(wrapper.Delete(uow, request));

        var handler = (IDeleteRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
    }

    [Fact]
    public void Retrieve_Wrappers_Throw_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncToAsyncRetrieveRequestProcessorWrapper<TestRow>(null));
        Assert.Throws<ArgumentNullException>(() => new AsyncToSyncRetrieveRequestProcessorWrapper<TestRow>(null));
    }

    [Fact]
    public async Task SyncToAsyncRetrieve_Wraps()
    {
        var mock = new FakeRetrieveProcessor<TestRow>();
        var wrapper = new SyncToAsyncRetrieveRequestProcessorWrapper<TestRow>(mock);
        var connection = new MockDbConnection();
        var request = new RetrieveRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(await wrapper.ProcessAsync(connection, request, TestContext.Current.CancellationToken));
        Assert.NotNull(await wrapper.RetrieveAsync(connection, request, TestContext.Current.CancellationToken));

        var handler = (IRetrieveRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
        Assert.True(handler.AllowSelectField(null));
        Assert.True(handler.ShouldSelectField(null));
        Assert.NotNull(handler.Request);
        Assert.Null(handler.Context);
    }

    [Fact]
    public void AsyncToSyncRetrieve_Wraps()
    {
        var mock = new FakeRetrieveProcessor<TestRow>();
        var wrapper = new AsyncToSyncRetrieveRequestProcessorWrapper<TestRow>(mock);
        var connection = new MockDbConnection();
        var request = new RetrieveRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(wrapper.Process(connection, request));
        Assert.NotNull(wrapper.Retrieve(connection, request));

        var handler = (IRetrieveRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.True(handler.AllowSelectField(null));
        Assert.True(handler.ShouldSelectField(null));
    }

    [Fact]
    public void Undelete_Wrappers_Throw_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncToAsyncUndeleteRequestProcessorWrapper<TestRow>(null));
        Assert.Throws<ArgumentNullException>(() => new AsyncToSyncUndeleteRequestProcessorWrapper<TestRow>(null));
    }

    [Fact]
    public async Task SyncToAsyncUndelete_Wraps()
    {
        var mock = new FakeUndeleteProcessor<TestRow>();
        var wrapper = new SyncToAsyncUndeleteRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new UndeleteRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(await wrapper.ProcessAsync(uow, request, TestContext.Current.CancellationToken));
        Assert.NotNull(await wrapper.UndeleteAsync(uow, request, TestContext.Current.CancellationToken));

        var handler = (IUndeleteRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
        Assert.Same(mock.StateBag, handler.StateBag);
        Assert.Same(mock.UnitOfWork, handler.UnitOfWork);
        Assert.NotNull(handler.Request);
        Assert.Null(handler.Context);
    }

    [Fact]
    public void AsyncToSyncUndelete_Wraps()
    {
        var mock = new FakeUndeleteProcessor<TestRow>();
        var wrapper = new AsyncToSyncUndeleteRequestProcessorWrapper<TestRow>(mock);
        var uow = Uow();
        var request = new UndeleteRequest();

        Assert.Same(mock, wrapper.WrappedHandler);
        Assert.NotNull(wrapper.Process(uow, request));
        Assert.NotNull(wrapper.Undelete(uow, request));

        var handler = (IUndeleteRequestHandler)wrapper;
        Assert.Same(mock.Row, handler.Row);
        Assert.Same(mock.Response, handler.Response);
    }
}
