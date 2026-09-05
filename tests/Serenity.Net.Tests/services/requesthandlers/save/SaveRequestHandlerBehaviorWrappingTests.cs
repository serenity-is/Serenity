using System.Threading;

namespace Serenity.Services;

public class SaveRequestHandlerBehaviorWrappingTests
{
    private class TestRow : IdNameRow<TestRow.RowFields>
    {
        public class RowFields : IdNameRowFields { }
    }

    private sealed class AsyncOnlySaveBehavior : ISaveBehaviorAsync
    {
        public bool OnBeforeSaveAsyncCalled;

        public Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
        {
            OnBeforeSaveAsyncCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class SyncOnlySaveBehavior : ISaveBehaviorSync
    {
        public bool OnBeforeSaveCalled;

        public void OnBeforeSave(ISaveRequestHandler handler)
        {
            OnBeforeSaveCalled = true;
        }
    }

    private sealed class MarkerOnlySaveBehavior : ISaveBehavior
    {
    }

    private static MockSaveHandler<TestRow> BuildHandler(MockDbConnection connection, IBehaviorProvider behaviors, bool isCreate = true)
    {
        var context = new NullRequestContext(behaviors).WithPermissions(x => true);
        return new MockSaveHandler<TestRow>
        {
            Row = new TestRow { Name = "Test" },
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Context = context,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection)
        };
    }

    [Fact]
    public void SyncHandler_WrapsAsyncBehavior()
    {
        var behavior = new AsyncOnlySaveBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => 1);
        var handler = new SaveRequestHandler<TestRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        handler.Create(new MockUnitOfWork(connection), new SaveRequest<TestRow>
        {
            Entity = new TestRow { Name = "Test" }
        });

        Assert.True(behavior.OnBeforeSaveAsyncCalled);
    }

    [Fact]
    public async Task AsyncHandler_WrapsSyncBehavior()
    {
        var behavior = new SyncOnlySaveBehavior();
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { behavior });

        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => 1);
        var handler = new SaveRequestHandlerAsync<TestRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<TestRow>
        {
            Entity = new TestRow { Name = "Test" }
        }, CancellationToken.None);

        Assert.True(behavior.OnBeforeSaveCalled);
    }

    [Fact]
    public void SyncHandler_Throws_ForMarkerOnlyBehavior()
    {
        var behaviors = new MockBehaviorProvider((handlerType, rowType, behaviorType) =>
            new object[] { new MarkerOnlySaveBehavior() });

        using var connection = new MockDbConnection();
        var handler = new SaveRequestHandler<TestRow>(
            new NullRequestContext(behaviors).WithPermissions(x => true));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Create(new MockUnitOfWork(connection), new SaveRequest<TestRow>
            {
                Entity = new TestRow { Name = "Test" }
            }));

        Assert.Contains("ISaveBehavior", exception.Message);
        Assert.Contains("ISaveBehaviorSync", exception.Message);
        Assert.Contains("ISaveBehaviorAsync", exception.Message);
    }
}
