using System.Threading;

namespace Serenity.Services;

public class RetrieveRequestHandlerBehaviorWrappingTests
{
    [TableName("RetRows")]
    private class RetRow : Row<RetRow.RowFields>, IIdRow
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

    private sealed class AsyncOnlyRetrieveBehavior : IRetrieveBehaviorAsync
    {
        public bool OnReturnAsyncCalled;

        public Task OnReturnAsync(IRetrieveRequestHandler handler, CancellationToken cancellationToken = default)
        {
            OnReturnAsyncCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class SyncOnlyRetrieveBehavior : IRetrieveBehaviorSync
    {
        public bool OnReturnCalled;

        public void OnReturn(IRetrieveRequestHandler handler)
        {
            OnReturnCalled = true;
        }
    }

    private sealed class MarkerOnlyRetrieveBehavior : IRetrieveBehavior
    {
    }

    private static MockDbConnection CreateConnection()
    {
        return new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 1 }));
    }

    [Fact]
    public void SyncHandler_WrapsAsyncBehavior()
    {
        var behavior = new AsyncOnlyRetrieveBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new RetrieveRequestHandler<RetRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        handler.Retrieve(connection, new RetrieveRequest { EntityId = 1 });

        Assert.True(behavior.OnReturnAsyncCalled);
    }

    [Fact]
    public async Task AsyncHandler_WrapsSyncBehavior()
    {
        var behavior = new SyncOnlyRetrieveBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new RetrieveRequestHandlerAsync<RetRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        await handler.RetrieveAsync(connection, new RetrieveRequest { EntityId = 1 },
            CancellationToken.None);

        Assert.True(behavior.OnReturnCalled);
    }

    [Fact]
    public void SyncHandler_Throws_ForMarkerOnlyBehavior()
    {
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { new MarkerOnlyRetrieveBehavior() });

        using var connection = CreateConnection();
        var handler = new RetrieveRequestHandler<RetRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Retrieve(connection, new RetrieveRequest { EntityId = 1 }));

        Assert.Contains("IRetrieveBehavior", exception.Message);
    }
}
