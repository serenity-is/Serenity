namespace Serenity.Services;

public partial class DeleteRequestHandlerTests
{
    [TableName("SoftRows")]
    private class SoftRow : Row<SoftRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [IdProperty]
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

    [TableName("ActiveRows")]
    private class ActiveRow : Row<ActiveRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int16Field IsActive;
#pragma warning restore CS0649
        }
    }

    [TableName("DeleteLogOnlyRows")]
    private class DeleteLogOnlyRow : Row<DeleteLogOnlyRow.RowFields>, IIdRow, IDeleteLogRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public DateTime? DeleteDate { get => fields.DeleteDate[this]; set => fields.DeleteDate[this] = value; }
        public long? DeleteUserId { get => fields.DeleteUserId[this]; set => fields.DeleteUserId[this] = value; }

        public DateTimeField DeleteDateField => fields.DeleteDate;
        public Field DeleteUserIdField => fields.DeleteUserId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public DateTimeField DeleteDate;
            public Int64Field DeleteUserId;
#pragma warning restore CS0649
        }
    }

    [TableName("SoftUpdateRows")]
    private class SoftUpdateLogRow : Row<SoftUpdateLogRow.RowFields>, IIdRow, IIsDeletedRow, IUpdateLogRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }
        public long? UpdateUserId { get => fields.UpdateUserId[this]; set => fields.UpdateUserId[this] = value; }

        public BooleanField IsDeletedField => fields.IsDeleted;
        public DateTimeField UpdateDateField => fields.UpdateDate;
        public Field UpdateUserIdField => fields.UpdateUserId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public BooleanField IsDeleted;
            public DateTimeField UpdateDate;
            public Int64Field UpdateUserId;
#pragma warning restore CS0649
        }
    }

    [TableName("SoftDeleteLogRows")]
    private class SoftDeleteLogRow : Row<SoftDeleteLogRow.RowFields>, IIdRow, IIsDeletedRow, IDeleteLogRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public DateTime? DeleteDate { get => fields.DeleteDate[this]; set => fields.DeleteDate[this] = value; }
        public long? DeleteUserId { get => fields.DeleteUserId[this]; set => fields.DeleteUserId[this] = value; }

        public BooleanField IsDeletedField => fields.IsDeleted;
        public DateTimeField DeleteDateField => fields.DeleteDate;
        public Field DeleteUserIdField => fields.DeleteUserId;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public BooleanField IsDeleted;
            public DateTimeField DeleteDate;
            public Int64Field DeleteUserId;
#pragma warning restore CS0649
        }
    }

    [TableName("DisplayOrderDelRows")]
    private class DisplayOrderDelRow : Row<DisplayOrderDelRow.RowFields>, IIdRow, IDisplayOrderRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public int? Order { get => fields.Order[this]; set => fields.Order[this] = value; }

        public Int32Field DisplayOrderField => fields.Order;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public Int32Field Order;
#pragma warning restore CS0649
        }
    }

    private sealed class DeleteExceptionBehavior : IDeleteBehaviorSync, IDeleteExceptionBehavior
    {
        public bool ExceptionCalled;

        public void OnException(IDeleteRequestHandler handler, Exception exception)
        {
            ExceptionCalled = true;
        }
    }

    private static IRequestContext CovDeleteContext() =>
        new NullRequestContext().WithPermissions(_ => true);

    private static MockDbConnection CovDeleteConnection() =>
        new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader())
            .InterceptExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);

    [Fact]
    public void SoftDelete_Sets_IsDeleted_And_UpdateLog()
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
        var handler = new DeleteRequestHandler<SoftUpdateLogRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
        Assert.False(response.WasAlreadyDeleted);
    }

    [Fact]
    public void SoftDelete_Sets_IsDeleted_And_DeleteLog()
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
        var handler = new DeleteRequestHandler<SoftDeleteLogRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void ActiveDelete_Sets_IsActive_MinusOne()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsActive = (short)1 }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<ActiveRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void DeleteLogOnly_Sets_DeleteUser_When_Not_Set()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new
            {
                Id = 5,
                DeleteDate = (DateTime?)null,
                DeleteUserId = (long?)null
            }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<DeleteLogOnlyRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Delete_AlreadyDeleted_Returns_WasAlreadyDeleted()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, IsDeleted = true }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<SoftRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.True(response.WasAlreadyDeleted);
    }

    [Fact]
    public void Delete_Throws_When_Entity_Not_Found_On_Load()
    {
        using var connection = CovDeleteConnection();
        var handler = new DeleteRequestHandler<SoftRow>(CovDeleteContext());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Delete_Throws_When_Hard_Delete_Affects_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 0);
        var handler = new DeleteRequestHandler<IdNameRow>(CovDeleteContext());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Delete_Throws_When_EntityId_Is_Null()
    {
        using var connection = CovDeleteConnection();
        var handler = new DeleteRequestHandler<SoftRow>(CovDeleteContext());

        Assert.Throws<ValidationError>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = null }));
    }

    [Fact]
    public void Delete_Invokes_Exception_Behavior_On_Error()
    {
        var behavior = new DeleteExceptionBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => throw new InvalidOperationException("boom"));
        var handler = new DeleteRequestHandler<IdNameRow>(
            new NullRequestContext(behaviors).WithPermissions(_ => true));

        Assert.Throws<InvalidOperationException>(() =>
            handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 }));
        Assert.True(behavior.ExceptionCalled);
    }

    [Fact]
    public void Delete_DisplayOrder_Row_Reorders_After_Delete()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Order = 1 }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new DeleteRequestHandler<DisplayOrderDelRow>(CovDeleteContext());

        var response = handler.Delete(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Delete_Explicit_Interface_Works()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { ID = 5, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 1);
        IDeleteRequestProcessor handler = new DeleteRequestHandler<IdNameRow>(CovDeleteContext());

        var response = handler.Process(new MockUnitOfWork(connection), new DeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }
}
