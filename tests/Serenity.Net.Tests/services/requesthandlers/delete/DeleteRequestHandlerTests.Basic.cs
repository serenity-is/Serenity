namespace Serenity.Services;

public class DeleteRequestHandlerTests_Basic
{
    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public void Delete_DeletesRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptManipulateRow(_ => 1)
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<IdNameRow>(Context());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Delete_When_No_Rows_Affected_Throws_EntityNotFound()
    {
        // Race: the row is found by the initial load, but the delete affects zero
        // rows (another request removed it in between). This must surface as a
        // friendly EntityNotFoundError, not a generic expected-rows exception.
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandler<IdNameRow>(Context());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Delete_MalformedEntityId_Throws_InvalidId()
    {
        using var connection = new MockDbConnection();
        var handler = new DeleteRequestHandler<IdNameRow>(Context());

        var ex = Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = "abc" }));
        Assert.Equal("InvalidId", ex.ErrorCode);
    }

    [Fact]
    public void Delete_UninitializedProperties_Throw()
    {
        var handler = new DeleteRequestHandler<IdNameRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
        Assert.Throws<InvalidOperationException>(() => handler.Response);
        Assert.Throws<InvalidOperationException>(() => handler.Connection);
        Assert.Throws<InvalidOperationException>(() => handler.UnitOfWork);
    }
}

