namespace Serenity.Services;

public partial class DeleteRequestHandlerTests
{
    [DeletePermission("Test:DeleteProtected")]
    private class ProtectedDeleteRow : Row<ProtectedDeleteRow.RowFields>, IIdRow
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

    [Fact]
    public void SoftDelete_Throws_When_Update_Affects_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsDeleted = false }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandler<SoftRow>(CovDeleteContext());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void DeleteLogOnly_Throws_When_Update_Affects_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                DeleteDate = (DateTime?)null,
                DeleteUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandler<DeleteLogOnlyRow>(CovDeleteContext());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public async Task DeleteAsync_SoftDelete_Throws_When_Update_Affects_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsDeleted = false }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandlerAsync<SoftRow>(CovDeleteContext());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_DeleteLogOnly_Throws_When_Update_Affects_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                DeleteDate = (DateTime?)null,
                DeleteUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandlerAsync<DeleteLogOnlyRow>(CovDeleteContext());

        await Assert.ThrowsAsync<ValidationError>(() =>
            handler.DeleteAsync(new MockUnitOfWork(connection),
                new DeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Delete_Throws_When_Permission_Denied()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<ProtectedDeleteRow>(
            new NullRequestContext().WithPermissions(_ => false));

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void AsyncToSyncWrapper_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 1);
        var async = new DeleteRequestHandlerAsync<IdNameRow>(CovDeleteContext());
        var wrapper = new AsyncToSyncDeleteRequestProcessorWrapper<IdNameRow>(async);
        var uow = new MockUnitOfWork(connection);

        var response = wrapper.Process(uow, new DeleteRequest { EntityId = 5 });
        Assert.NotNull(response);
        Assert.NotNull(wrapper.Delete(uow, new DeleteRequest { EntityId = 5 }));

        var handler = (IDeleteRequestHandler)wrapper;
        Assert.NotNull(handler.Row);
        Assert.NotNull(handler.Request);
        Assert.NotNull(handler.Response);
        Assert.NotNull(handler.StateBag);
        Assert.NotNull(handler.Connection);
        Assert.NotNull(handler.UnitOfWork);
        Assert.NotNull(handler.Context);
    }

    [Fact]
    public void AsyncToSyncWrapper_Throws_For_Null_Handler()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AsyncToSyncDeleteRequestProcessorWrapper<IdNameRow>(null!));
    }
}
