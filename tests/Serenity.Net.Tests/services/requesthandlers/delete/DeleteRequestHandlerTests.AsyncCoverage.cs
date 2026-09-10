namespace Serenity.Services;

public partial class DeleteRequestHandlerTests
{
    private sealed class DeleteExceptionBehaviorAsync : IDeleteBehaviorAsync, IDeleteExceptionBehavior
    {
        public bool ExceptionCalled;

        public void OnException(IDeleteRequestHandler handler, Exception exception)
        {
            ExceptionCalled = true;
        }
    }

    [Fact]
    public async Task DeleteAsync_SoftUpdateLog()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                IsDeleted = false,
                UpdateDate = (DateTime?)null,
                UpdateUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandlerAsync<SoftUpdateLogRow>(CovDeleteContext());

        var response = await handler.DeleteAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DeleteAsync_ActiveDelete()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsActive = (short)1 }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandlerAsync<ActiveRow>(CovDeleteContext());

        var response = await handler.DeleteAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeleteLog()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                IsDeleted = false,
                DeleteDate = (DateTime?)null,
                DeleteUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandlerAsync<SoftDeleteLogRow>(CovDeleteContext());

        var response = await handler.DeleteAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DeleteAsync_DeleteLogOnly()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                DeleteDate = (DateTime?)null,
                DeleteUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandlerAsync<DeleteLogOnlyRow>(CovDeleteContext());

        var response = await handler.DeleteAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DeleteAsync_AlreadyDeleted()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsDeleted = true }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandlerAsync<SoftRow>(CovDeleteContext());

        var response = await handler.DeleteAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.True(response.WasAlreadyDeleted);
    }

    [Fact]
    public async Task DeleteAsync_Throws_When_Not_Found()
    {
        using var connection = CovDeleteConnection();
        var handler = new DeleteRequestHandlerAsync<SoftRow>(CovDeleteContext());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_Throws_When_Hard_Delete_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandlerAsync<IdNameRow>(CovDeleteContext());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_Throws_For_Null_EntityId()
    {
        using var connection = CovDeleteConnection();
        var handler = new DeleteRequestHandlerAsync<SoftRow>(CovDeleteContext());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = null }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_Explicit_Interface_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 1);
        IDeleteRequestProcessorAsync handler = new DeleteRequestHandlerAsync<IdNameRow>(CovDeleteContext());

        var response = await handler.ProcessAsync(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DeleteAsync_Invokes_Exception_Behavior()
    {
        var behavior = new DeleteExceptionBehaviorAsync();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => throw new InvalidOperationException("boom"));
        var handler = new DeleteRequestHandlerAsync<IdNameRow>(
            new NullRequestContext(behaviors).WithPermissions(_ => true));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken));
        Assert.True(behavior.ExceptionCalled);
    }
}
