using System.Threading;

namespace Serenity.Services;

public class ListRequestHandlerBehaviorWrappingTests
{
    [TableName("ListRows")]
    private class ListRow : Row<ListRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
#pragma warning restore CS0649
        }
    }

    private sealed class AsyncOnlyListBehavior : IListBehaviorAsync
    {
        public bool OnReturnAsyncCalled;

        public Task OnReturnAsync(IListRequestHandler handler, CancellationToken cancellationToken = default)
        {
            OnReturnAsyncCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class SyncOnlyListBehavior : IListBehaviorSync
    {
        public bool OnReturnCalled;

        public void OnReturn(IListRequestHandler handler)
        {
            OnReturnCalled = true;
        }
    }

    private sealed class MarkerOnlyListBehavior : IListBehavior
    {
    }

    private static MockDbConnection CreateConnection()
    {
        return new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader());
    }

    [Fact]
    public void SyncHandler_WrapsAsyncBehavior()
    {
        var behavior = new AsyncOnlyListBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new ListRequestHandler<ListRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        handler.List(connection, new ListRequest());

        Assert.True(behavior.OnReturnAsyncCalled);
    }

    [Fact]
    public async Task AsyncHandler_WrapsSyncBehavior()
    {
        var behavior = new SyncOnlyListBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new ListRequestHandlerAsync<ListRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        await handler.ListAsync(connection, new ListRequest(), CancellationToken.None);

        Assert.True(behavior.OnReturnCalled);
    }

    [Fact]
    public void SyncHandler_Throws_ForMarkerOnlyBehavior()
    {
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { new MarkerOnlyListBehavior() });

        using var connection = CreateConnection();
        var handler = new ListRequestHandler<ListRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.List(connection, new ListRequest()));

        Assert.Contains("IListBehavior", exception.Message);
    }
}
