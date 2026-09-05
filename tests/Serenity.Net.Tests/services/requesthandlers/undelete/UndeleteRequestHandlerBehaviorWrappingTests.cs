using System.Threading;

namespace Serenity.Services;

public class UndeleteRequestHandlerBehaviorWrappingTests
{
    [TableName("UndelRows")]
    private class UndelRow : Row<UndelRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }

        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public BooleanField IsDeleted;
#pragma warning restore CS0649
        }
    }

    private sealed class AsyncOnlyUndeleteBehavior : IUndeleteBehaviorAsync
    {
        public bool OnBeforeUndeleteAsyncCalled;

        public Task OnBeforeUndeleteAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
        {
            OnBeforeUndeleteAsyncCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class SyncOnlyUndeleteBehavior : IUndeleteBehaviorSync
    {
        public bool OnBeforeUndeleteCalled;

        public void OnBeforeUndelete(IUndeleteRequestHandler handler)
        {
            OnBeforeUndeleteCalled = true;
        }
    }

    private sealed class MarkerOnlyUndeleteBehavior : IUndeleteBehavior
    {
    }

    private static MockDbConnection CreateConnection()
    {
        return new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 1, IsDeleted = true }))
            .InterceptExecuteNonQuery(args => 1);
    }

    [Fact]
    public void SyncHandler_WrapsAsyncBehavior()
    {
        var behavior = new AsyncOnlyUndeleteBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new UndeleteRequestHandler<UndelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 1 });

        Assert.True(behavior.OnBeforeUndeleteAsyncCalled);
    }

    [Fact]
    public async Task AsyncHandler_WrapsSyncBehavior()
    {
        var behavior = new SyncOnlyUndeleteBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = CreateConnection();
        var handler = new UndeleteRequestHandlerAsync<UndelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        await handler.UndeleteAsync(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 1 },
            CancellationToken.None);

        Assert.True(behavior.OnBeforeUndeleteCalled);
    }

    [Fact]
    public void SyncHandler_Throws_ForMarkerOnlyBehavior()
    {
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { new MarkerOnlyUndeleteBehavior() });

        using var connection = CreateConnection();
        var handler = new UndeleteRequestHandler<UndelRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 1 }));

        Assert.Contains("IUndeleteBehavior", exception.Message);
    }
}
