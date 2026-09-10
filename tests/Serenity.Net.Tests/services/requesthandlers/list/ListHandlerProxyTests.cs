namespace Serenity.Services;

public class ListHandlerProxyTests
{
    [TableName("ProxyListRows")]
    private class ProxyRow : Row<ProxyRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    private static IRequestContext Context() =>
        new NullRequestContext().WithPermissions(_ => true);

    private static MockDbConnection Connection() =>
        new MockDbConnection().InterceptExecuteReader(_ => new MockDbDataReader());

    [Fact]
    public void ListHandlerProxy_Lists()
    {
        using var connection = Connection();
        var factory = new MockHandlerFactory((_, _) => new ListRequestHandler<ProxyRow>(Context()));
        var proxy = new ListHandlerProxy<ProxyRow>(factory);

        var response = proxy.List(connection, new ListRequest());

        Assert.NotNull(response);
    }

    [Fact]
    public void ListHandlerProxy_WithRequestType_Lists()
    {
        using var connection = Connection();
        var factory = new MockHandlerFactory((_, _) => new ListRequestHandler<ProxyRow>(Context()));
        var proxy = new ListHandlerProxy<ProxyRow, ListRequest>(factory);

        var response = proxy.List(connection, new ListRequest());

        Assert.NotNull(response);
    }

    [Fact]
    public void ListHandlerProxy_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new ListHandlerProxy<ProxyRow>(null!));
    }

    [Fact]
    public async Task ListHandlerProxyAsync_Lists()
    {
        using var connection = Connection();
        var factory = new MockHandlerFactory((_, _) => new ListRequestHandlerAsync<ProxyRow>(Context()));
        var proxy = new ListHandlerProxyAsync<ProxyRow>(factory);

        var response = await proxy.ListAsync(connection, new ListRequest(),
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task ListHandlerProxyAsync_WithRequestType_Lists()
    {
        using var connection = Connection();
        var factory = new MockHandlerFactory((_, _) => new ListRequestHandlerAsync<ProxyRow>(Context()));
        var proxy = new ListHandlerProxyAsync<ProxyRow, ListRequest>(factory);

        var response = await proxy.ListAsync(connection, new ListRequest(),
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public void ListHandlerProxyAsync_Throws_For_Null_Factory()
    {
        Assert.Throws<ArgumentNullException>(() => new ListHandlerProxyAsync<ProxyRow>(null!));
    }

    [Fact]
    public void AsyncToSyncWrapper_Process_And_List()
    {
        using var connection = Connection();
        var async = new ListRequestHandlerAsync<ProxyRow>(Context());
        var wrapper = new AsyncToSyncListRequestProcessorWrapper<ProxyRow>(async);

        var response = wrapper.Process(connection, new ListRequest());
        Assert.NotNull(response);

        var list = wrapper.List(connection, new ListRequest());
        Assert.NotNull(list);

        var handler = (IListRequestHandler)wrapper;
        Assert.NotNull(handler.Row);
        Assert.NotNull(handler.Request);
        Assert.NotNull(handler.Response);
        Assert.NotNull(handler.StateBag);
        Assert.NotNull(handler.Connection);
        Assert.NotNull(handler.Context);
        Assert.True(handler.AllowSelectField(ProxyRow.Fields.Id));
        Assert.True(handler.ShouldSelectField(ProxyRow.Fields.Id));
        handler.IgnoreEqualityFilter(ProxyRow.Fields.Id.Name);
    }

    [Fact]
    public void AsyncToSyncWrapper_Throws_For_Null_Handler()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AsyncToSyncListRequestProcessorWrapper<ProxyRow>(null!));
    }
}
