using Serenity.TestUtils;

namespace Serenity.Services;

public class EntityNotFoundOracleTests
{
    private const string InsertPerm = "Test:Insert";
    private const string DeletePerm = "Test:Delete";
    private const string UpdatePerm = "Test:Update";

    [InsertPermission(InsertPerm)]
    [DeletePermission(DeletePerm)]
    [UpdatePermission(UpdatePerm)]
    private class OracleRow : IdNameRow<OracleRow.RowFields>
    {
        public class RowFields : IdNameRowFields { }
    }

    private static MockDbConnection ConnectionWithRow() =>
        new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "A" }));

    private static MockDbConnection ConnectionWithoutRow() =>
        new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader());

    private static IRequestContext Denied() =>
        new NullRequestContext().WithPermissions(_ => false);

    private static IRequestContext Allowed() =>
        new NullRequestContext().WithPermissions(_ => true);

    private static void AssertIndistinguishable(ValidationError missing, ValidationError denied)
    {
        Assert.Equal("EntityNotFound", missing.ErrorCode);
        Assert.Equal(missing.ErrorCode, denied.ErrorCode);
        Assert.Equal(missing.Message, denied.Message);
        Assert.Equal(missing.Arguments, denied.Arguments);
    }

    [Fact]
    public void Delete_Does_Not_Reveal_Whether_Record_Exists()
    {
        var missing = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithoutRow();
            new DeleteRequestHandler<OracleRow>(Denied())
                .Delete(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 });
        });

        var denied = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithRow();
            new DeleteRequestHandler<OracleRow>(Denied())
                .Delete(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 });
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public async Task DeleteAsync_Does_Not_Reveal_Whether_Record_Exists()
    {
        var ct = TestContext.Current.CancellationToken;

        var missing = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithoutRow();
            await new DeleteRequestHandlerAsync<OracleRow>(Denied())
                .DeleteAsync(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 }, ct);
        });

        var denied = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithRow();
            await new DeleteRequestHandlerAsync<OracleRow>(Denied())
                .DeleteAsync(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 }, ct);
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public void Undelete_Does_Not_Reveal_Whether_Record_Exists()
    {
        var missing = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithoutRow();
            new UndeleteRequestHandler<OracleRow>(Denied())
                .Undelete(new MockUnitOfWork(conn), new UndeleteRequest { EntityId = 5 });
        });

        var denied = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithRow();
            new UndeleteRequestHandler<OracleRow>(Denied())
                .Undelete(new MockUnitOfWork(conn), new UndeleteRequest { EntityId = 5 });
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public async Task UndeleteAsync_Does_Not_Reveal_Whether_Record_Exists()
    {
        var ct = TestContext.Current.CancellationToken;

        var missing = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithoutRow();
            await new UndeleteRequestHandlerAsync<OracleRow>(Denied())
                .UndeleteAsync(new MockUnitOfWork(conn), new UndeleteRequest { EntityId = 5 }, ct);
        });

        var denied = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithRow();
            await new UndeleteRequestHandlerAsync<OracleRow>(Denied())
                .UndeleteAsync(new MockUnitOfWork(conn), new UndeleteRequest { EntityId = 5 }, ct);
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public void Save_Update_Does_Not_Reveal_Whether_Record_Exists()
    {
        var missing = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithoutRow();
            new SaveRequestHandler<OracleRow>(Denied())
                .Update(new MockUnitOfWork(conn), new() { EntityId = 5, Entity = new OracleRow { Id = 5 } });
        });

        var denied = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithRow();
            new SaveRequestHandler<OracleRow>(Denied())
                .Update(new MockUnitOfWork(conn), new() { EntityId = 5, Entity = new OracleRow { Id = 5 } });
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public async Task Save_UpdateAsync_Does_Not_Reveal_Whether_Record_Exists()
    {
        var ct = TestContext.Current.CancellationToken;

        var missing = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithoutRow();
            await new SaveRequestHandlerAsync<OracleRow>(Denied())
                .UpdateAsync(new MockUnitOfWork(conn), new() { EntityId = 5, Entity = new OracleRow { Id = 5 } }, ct);
        });

        var denied = await Assert.ThrowsAsync<ValidationError>(async () =>
        {
            using var conn = ConnectionWithRow();
            await new SaveRequestHandlerAsync<OracleRow>(Denied())
                .UpdateAsync(new MockUnitOfWork(conn), new() { EntityId = 5, Entity = new OracleRow { Id = 5 } }, ct);
        });

        AssertIndistinguishable(missing, denied);
    }

    [Fact]
    public void Save_Create_Still_Reports_AccessDenied()
    {
        var error = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithoutRow();
            new SaveRequestHandler<OracleRow>(Denied())
                .Create(new MockUnitOfWork(conn), new() { Entity = new OracleRow { Name = "A" } });
        });

        Assert.Equal("AccessDenied", error.ErrorCode);
    }

    [Fact]
    public void Delete_Allowed_And_Missing_Reports_EntityNotFound()
    {
        var error = Assert.Throws<ValidationError>(() =>
        {
            using var conn = ConnectionWithoutRow();
            new DeleteRequestHandler<OracleRow>(Allowed())
                .Delete(new MockUnitOfWork(conn), new DeleteRequest { EntityId = 5 });
        });

        Assert.Equal("EntityNotFound", error.ErrorCode);
    }
}
