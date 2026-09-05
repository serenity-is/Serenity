using System.Globalization;
using System.Threading;

namespace Serenity.Services;

public class CaptureLogBehaviorTests
{
    [CaptureLog(typeof(MyLogRow), MappedIdField = "LogId")]
    [TableName("MyRows")]
    private class MyRow : Row<MyRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    [TableName("MyLog")]
    private class MyLogRow : Row<MyLogRow.RowFields>, ICaptureLogRow
    {
        [Identity]
        public long? LogId { get => fields.LogId[this]; set => fields.LogId[this] = value; }

        public CaptureOperationType? OperationType { get => fields.OperationType[this]; set => fields.OperationType[this] = value; }
        public int? ChangingUserId { get => fields.ChangingUserId[this]; set => fields.ChangingUserId[this] = value; }
        public DateTime? ValidFrom { get => fields.ValidFrom[this]; set => fields.ValidFrom[this] = value; }
        public DateTime? ValidUntil { get => fields.ValidUntil[this]; set => fields.ValidUntil[this] = value; }

        EnumField<CaptureOperationType> ICaptureLogRow.OperationTypeField => fields.OperationType;
        Field ICaptureLogRow.ChangingUserIdField => fields.ChangingUserId;
        DateTimeField ICaptureLogRow.ValidFromField => fields.ValidFrom;
        DateTimeField ICaptureLogRow.ValidUntilField => fields.ValidUntil;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int64Field LogId;
            public EnumField<CaptureOperationType> OperationType;
            public Int32Field ChangingUserId;
            public DateTimeField ValidFrom;
            public DateTimeField ValidUntil;
#pragma warning restore CS0649
        }
    }

    private const string adminId = "123456";

    private static MockDbConnection CreateConnection()
    {
        return new MockDbConnection()
            .InterceptExecuteNonQuery(args => 1)
            .InterceptManipulateRow(args => 1);
    }

    private static MockSaveHandler<MyRow> CreateSaveHandler(MockDbConnection connection, bool isCreate, MyRow row)
    {
        return new MockSaveHandler<MyRow>
        {
            Row = row,
            Old = isCreate ? null : new MyRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = new NullRequestContext(userAccessor: new MockUserAccessor(
                getUsername: () => "admin", getIdentifier: () => adminId))
        };
    }

    private static CaptureLogBehavior CreateBehavior(MyRow row)
    {
        var behavior = new CaptureLogBehavior();
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    [Fact]
    public void OnAudit_Sync_Create_LogsInsert()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Test" });
        using var connection = CreateConnection();
        var handler = CreateSaveHandler(connection, true, new MyRow { Id = 1, Name = "Test" });

        behavior.OnAudit(handler);

        // close-active SqlUpdate + one insert for the create operation
        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.False(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Single(connection.ManipulateRowCalls);
        Assert.False(connection.ManipulateRowCalls[0].IsAsync);
    }

    [Fact]
    public async Task OnAuditAsync_Async_Create_LogsInsert()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Test" });
        using var connection = CreateConnection();
        var handler = CreateSaveHandler(connection, true, new MyRow { Id = 1, Name = "Test" });

        await behavior.OnAuditAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.True(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Single(connection.ManipulateRowCalls);
        Assert.True(connection.ManipulateRowCalls[0].IsAsync);
    }

    [Fact]
    public void OnAudit_Sync_Update_LogsBeforeAndUpdate()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Old" });
        using var connection = CreateConnection();
        var handler = CreateSaveHandler(connection, false, new MyRow { Id = 1, Name = "New" });
        handler.Old = new MyRow { Id = 1, Name = "Old" };

        behavior.OnAudit(handler);

        // close-active SqlUpdate + Before insert + Update insert
        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.False(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Equal(2, connection.ManipulateRowCalls.Count);
        Assert.All(connection.ManipulateRowCalls, x => Assert.False(x.IsAsync));
    }

    [Fact]
    public async Task OnAuditAsync_Async_Update_LogsBeforeAndUpdate()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Old" });
        using var connection = CreateConnection();
        var handler = CreateSaveHandler(connection, false, new MyRow { Id = 1, Name = "New" });
        handler.Old = new MyRow { Id = 1, Name = "Old" };

        await behavior.OnAuditAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.True(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Equal(2, connection.ManipulateRowCalls.Count);
        Assert.All(connection.ManipulateRowCalls, x => Assert.True(x.IsAsync));
    }

    [Fact]
    public void OnAudit_Sync_Delete_LogsDelete()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Test" });
        using var connection = CreateConnection();
        var handler = new MockDeleteHandler<MyRow>
        {
            Row = new MyRow { Id = 1, Name = "Test" },
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = new NullRequestContext(userAccessor: new MockUserAccessor(
                getUsername: () => "admin", getIdentifier: () => adminId))
        };

        behavior.OnAudit(handler);

        // close-active SqlUpdate + one insert (the delete "Before" snapshot)
        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.False(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Single(connection.ManipulateRowCalls);
        Assert.False(connection.ManipulateRowCalls[0].IsAsync);
    }

    [Fact]
    public async Task OnAuditAsync_Async_Delete_LogsDelete()
    {
        var behavior = CreateBehavior(new MyRow { Id = 1, Name = "Test" });
        using var connection = CreateConnection();
        var handler = new MockDeleteHandler<MyRow>
        {
            Row = new MyRow { Id = 1, Name = "Test" },
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = new NullRequestContext(userAccessor: new MockUserAccessor(
                getUsername: () => "admin", getIdentifier: () => adminId))
        };

        await behavior.OnAuditAsync(handler, CancellationToken.None);

        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.True(connection.ExecuteNonQueryCalls[0].IsAsync);
        Assert.Single(connection.ManipulateRowCalls);
        Assert.True(connection.ManipulateRowCalls[0].IsAsync);
    }
}
