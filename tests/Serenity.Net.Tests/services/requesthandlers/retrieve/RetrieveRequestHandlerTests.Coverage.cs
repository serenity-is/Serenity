namespace Serenity.Services;

public class RetrieveRequestHandlerTests_Coverage
{
#pragma warning disable CS0649
    [TableName("CoverRetRows")]
    private class CoverRow : Row<CoverRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }
#pragma warning restore CS0649

    private sealed class ExceptionBehavior : IRetrieveBehaviorSync, IRetrieveExceptionBehavior
    {
        public bool OnExceptionCalled;

        public void OnException(IRetrieveRequestHandler handler, Exception exception)
            => OnExceptionCalled = true;
    }

    private sealed class AsyncExceptionBehavior : IRetrieveBehaviorAsync, IRetrieveExceptionBehavior
    {
        public bool OnExceptionCalled;

        public void OnException(IRetrieveRequestHandler handler, Exception exception)
            => OnExceptionCalled = true;
    }

    private static IRequestContext Context(MockBehaviorProvider? behaviors = null)
        => new NullRequestContext(behaviors).WithPermissions(_ => true);

    [Fact]
    public void Process_Throws_ForNullEntityId()
    {
        using var connection = new MockDbConnection();
        var handler = new RetrieveRequestHandler<CoverRow>(Context());

        Assert.Throws<ValidationError>(() =>
            handler.Process(connection, new RetrieveRequest { EntityId = null }));
    }

    [Fact]
    public void Process_Throws_EntityNotFound_And_Calls_ExceptionBehavior()
    {
        var behavior = new ExceptionBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new RetrieveRequestHandler<CoverRow>(Context(behaviors));

        Assert.Throws<ValidationError>(() =>
            handler.Process(connection, new RetrieveRequest { EntityId = 5 }));
        Assert.True(behavior.OnExceptionCalled);
    }

    [Fact]
    public void Explicit_Processor_Interface_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }));
        var handler = new RetrieveRequestHandler<CoverRow>(Context());

        var response = ((IRetrieveRequestProcessor)handler)
            .Process(connection, new RetrieveRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public async Task ProcessAsync_Throws_ForNullEntityId()
    {
        using var connection = new MockDbConnection();
        var handler = new RetrieveRequestHandlerAsync<CoverRow>(Context());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.ProcessAsync(connection, new RetrieveRequest { EntityId = null },
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ProcessAsync_Throws_EntityNotFound_And_Calls_ExceptionBehavior()
    {
        var behavior = new AsyncExceptionBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });

        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new RetrieveRequestHandlerAsync<CoverRow>(Context(behaviors));

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.ProcessAsync(connection, new RetrieveRequest { EntityId = 5 },
                TestContext.Current.CancellationToken));
        Assert.True(behavior.OnExceptionCalled);
    }

    [Fact]
    public async Task Explicit_Async_Processor_Interface_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }));
        var handler = new RetrieveRequestHandlerAsync<CoverRow>(Context());

        var response = await ((IRetrieveRequestProcessorAsync)handler)
            .ProcessAsync(connection, new RetrieveRequest { EntityId = 5 },
                TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }
}
