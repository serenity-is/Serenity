namespace Serenity.Services;

public class ListRequestHandlerAsyncTests
{
    [TableName("AsyncListRows")]
    private class AsyncRow : Row<AsyncRow.RowFields>, IIdRow
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

    [ReadPermission("Async:Read")]
    private class ProtectedAsyncRow : Row<ProtectedAsyncRow.RowFields>, IIdRow
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

    private sealed class SkippingAsyncHandler : ListRequestHandlerAsync<AsyncRow>
    {
        public SkippingAsyncHandler(IRequestContext context) : base(context) { }

        protected override AsyncRow ProcessEntity(AsyncRow row) => null!;
    }

    private sealed class ExceptionAsyncBehavior : IListBehaviorAsync, IListExceptionBehavior
    {
        public bool ExceptionCalled;

        public void OnException(IListRequestHandler handler, Exception exception)
        {
            ExceptionCalled = true;
        }
    }

    private static IRequestContext Context() =>
        new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public async Task ListAsync_Returns_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1 }));
        var handler = new ListRequestHandlerAsync<AsyncRow>(Context());

        var response = await handler.ListAsync(connection, new ListRequest(),
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Entities);
    }

    [Fact]
    public async Task ListAsync_Skips_Entity_When_ProcessEntity_Returns_Null()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1 }));
        var handler = new SkippingAsyncHandler(Context());

        var response = await handler.ListAsync(connection, new ListRequest(),
            TestContext.Current.CancellationToken);

        Assert.Empty(response.Entities);
    }

    [Fact]
    public async Task ListAsync_With_Valid_Distinct_Fields_Populates_Values()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1 }));
        var handler = new ListRequestHandlerAsync<AsyncRow>(Context());

        var response = await handler.ListAsync(connection, new ListRequest
        {
            DistinctFields = [new SortBy(AsyncRow.Fields.Id.Name, false)]
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(response.Values);
    }

    [Fact]
    public async Task ListAsync_With_Invalid_Distinct_Fields_Marks_Values_Null()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new ListRequestHandlerAsync<AsyncRow>(Context());

        var response = await handler.ListAsync(connection, new ListRequest
        {
            DistinctFields = [new SortBy("NoSuchField", false)]
        }, TestContext.Current.CancellationToken);

        Assert.Null(response.Values);
    }

    [Fact]
    public async Task ProcessAsync_Explicit_Interface_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        IListRequestProcessorAsync handler = new ListRequestHandlerAsync<AsyncRow>(Context());

        var response = await handler.ProcessAsync(connection, new ListRequest(),
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task ListAsync_Throws_For_Null_Arguments()
    {
        var handler = new ListRequestHandlerAsync<AsyncRow>(Context());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.ListAsync(null!, new ListRequest(), TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.ListAsync(new MockDbConnection(), null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListAsync_Validates_Permissions()
    {
        var context = new NullRequestContext().WithPermissions(_ => false);
        var handler = new ListRequestHandlerAsync<ProtectedAsyncRow>(context);

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.ListAsync(new MockDbConnection(), new ListRequest(),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListAsync_Invokes_Exception_Behavior_On_Error()
    {
        var behavior = new ExceptionAsyncBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => throw new InvalidOperationException("boom"));
        var handler = new ListRequestHandlerAsync<AsyncRow>(
            new NullRequestContext(behaviors).WithPermissions(_ => true));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.ListAsync(connection, new ListRequest(), TestContext.Current.CancellationToken));

        Assert.True(behavior.ExceptionCalled);
    }
}
