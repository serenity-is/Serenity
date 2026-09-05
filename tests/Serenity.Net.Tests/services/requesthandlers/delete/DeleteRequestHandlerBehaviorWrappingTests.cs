using System.Threading;

namespace Serenity.Services;

public class DeleteRequestHandlerBehaviorWrappingTests
{
    [TableName("DelRows")]
    private class DelRow : Row<DelRow.RowFields>, IIdRow
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

    private sealed class AsyncOnlyDeleteBehavior : IDeleteBehaviorAsync
    {
        public bool OnBeforeDeleteAsyncCalled;

        public Task OnBeforeDeleteAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
        {
            OnBeforeDeleteAsyncCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class SyncOnlyDeleteBehavior : IDeleteBehaviorSync
    {
        public bool OnBeforeDeleteCalled;

        public void OnBeforeDelete(IDeleteRequestHandler handler)
        {
            OnBeforeDeleteCalled = true;
        }
    }

    private sealed class MarkerOnlyDeleteBehavior : IDeleteBehavior
    {
    }

    private static MockDbConnection CreateConnection()
    {
        return new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 1 }))
            .InterceptExecuteNonQuery(args => 1);
    }

    [Fact]
    public void SyncHandler_WrapsAsyncBehavior()
    {
        var behavior = new AsyncOnlyDeleteBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new DeleteRequestHandler<DelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 1 });

        Assert.True(behavior.OnBeforeDeleteAsyncCalled);
    }

    [Fact]
    public async Task AsyncHandler_WrapsSyncBehavior()
    {
        var behavior = new SyncOnlyDeleteBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new DeleteRequestHandlerAsync<DelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        await handler.DeleteAsync(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 1 },
            CancellationToken.None);

        Assert.True(behavior.OnBeforeDeleteCalled);
    }

    [Fact]
    public void SyncHandler_Throws_ForMarkerOnlyBehavior()
    {
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { new MarkerOnlyDeleteBehavior() });

        using var connection = CreateConnection();
        var handler = new DeleteRequestHandler<DelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 1 }));

        Assert.Contains("IDeleteBehavior", exception.Message);
    }
}
