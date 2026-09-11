#pragma warning disable CS0649
namespace Serenity.Services;

public partial class CaptureLogBehaviorTests
{
    [TableName("NoAttrRows")]
    private class NoAttrRow : Row<NoAttrRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    private class NotIdRow : Row<NotIdRow.RowFields>
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [CaptureLog(typeof(MyLogRow), MappedIdField = "LogId")]
    [TableName("DeletedAuditRows")]
    private class DeletedAuditRow : Row<DeletedAuditRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public BooleanField IsDeleted;
        }
    }

    [CaptureLog(typeof(MyLogRow), MappedIdField = "LogId")]
    [TableName("ActiveDeletedAuditRows")]
    private class ActiveDeletedAuditRow : Row<ActiveDeletedAuditRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int16Field IsActive;
        }
    }

    [CaptureLog(typeof(MyLogRow), MappedIdField = "LogId")]
    [TableName("DeleteLogAuditRows")]
    private class DeleteLogAuditRow : Row<DeleteLogAuditRow.RowFields>, IIdRow, IDeleteLogRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? DeleteUserId { get => fields.DeleteUserId[this]; set => fields.DeleteUserId[this] = value; }
        public DateTime? DeleteDate { get => fields.DeleteDate[this]; set => fields.DeleteDate[this] = value; }
        public Field DeleteUserIdField => fields.DeleteUserId;
        public DateTimeField DeleteDateField => fields.DeleteDate;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field DeleteUserId;
            public DateTimeField DeleteDate;
        }
    }

    [CaptureLog(typeof(MyLogRow), MappedIdField = "LogId")]
    [TableName("AuditRows")]
    private class AuditRow : Row<AuditRow.RowFields>, IIdRow,
        IInsertDateRow, IInsertUserIdRow, IUpdateDateRow, IUpdateUserIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }
        public int? InsertUserId { get => fields.InsertUserId[this]; set => fields.InsertUserId[this] = value; }
        public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }
        public int? UpdateUserId { get => fields.UpdateUserId[this]; set => fields.UpdateUserId[this] = value; }
        public DateTimeField InsertDateField => fields.InsertDate;
        public Field InsertUserIdField => fields.InsertUserId;
        public DateTimeField UpdateDateField => fields.UpdateDate;
        public Field UpdateUserIdField => fields.UpdateUserId;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public DateTimeField InsertDate;
            public Int32Field InsertUserId;
            public DateTimeField UpdateDate;
            public Int32Field UpdateUserId;
        }
    }

    [TableName("PlainLogs")]
    private class NotCaptureLogRow : Row<NotCaptureLogRow.RowFields>
    {
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [CaptureLog(typeof(NotCaptureLogRow))]
    [TableName("BadLogMasters")]
    private class BadLogMasterRow : Row<BadLogMasterRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [CaptureLog(typeof(MyLogRow), MappedIdField = "Nope")]
    [TableName("BadMappedMasters")]
    private class BadMappedMasterRow : Row<BadMappedMasterRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("MismatchLogs")]
    private class MismatchLogRow : Row<MismatchLogRow.RowFields>, ICaptureLogRow
    {
        [Identity]
        public long? LogId { get => fields.LogId[this]; set => fields.LogId[this] = value; }
        public CaptureOperationType? OperationType { get => fields.OperationType[this]; set => fields.OperationType[this] = value; }
        public int? ChangingUserId { get => fields.ChangingUserId[this]; set => fields.ChangingUserId[this] = value; }
        public DateTime? ValidFrom { get => fields.ValidFrom[this]; set => fields.ValidFrom[this] = value; }
        public DateTime? ValidUntil { get => fields.ValidUntil[this]; set => fields.ValidUntil[this] = value; }
        public string Foo { get => fields.Foo[this]; set => fields.Foo[this] = value; }

        EnumField<CaptureOperationType> ICaptureLogRow.OperationTypeField => fields.OperationType;
        Field ICaptureLogRow.ChangingUserIdField => fields.ChangingUserId;
        DateTimeField ICaptureLogRow.ValidFromField => fields.ValidFrom;
        DateTimeField ICaptureLogRow.ValidUntilField => fields.ValidUntil;

        public class RowFields : RowFieldsBase
        {
            public Int64Field LogId;
            public EnumField<CaptureOperationType> OperationType;
            public Int32Field ChangingUserId;
            public DateTimeField ValidFrom;
            public DateTimeField ValidUntil;
            public StringField Foo;
        }
    }

    [CaptureLog(typeof(MismatchLogRow), MappedIdField = "LogId")]
    [TableName("MismatchMasters")]
    private class MismatchMasterRow : Row<MismatchMasterRow.RowFields>, IIdRow
    {
        [Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
        }
    }

    private class FakeUndeleteHandler<TRow> : IUndeleteRequestHandler where TRow : IRow, new()
    {
        public IRow Row { get; set; } = new TRow();
        public UndeleteRequest Request { get; set; } = new();
        public UndeleteResponse Response { get; set; } = new();
        public IDictionary<string, object> StateBag { get; set; } = new Dictionary<string, object>();
        public IDbConnection? Connection { get; set; }
        public IUnitOfWork? UnitOfWork { get; set; }
        public IRequestContext? Context { get; set; }
    }

    private static IRequestContext Context() =>
        new NullRequestContext(userAccessor: new MockUserAccessor(
            getUsername: () => "admin", getIdentifier: () => adminId));

    private static CaptureLogBehavior CreateBehaviorFor<T>(T row) where T : IRow
    {
        var behavior = new CaptureLogBehavior();
        Assert.True(behavior.ActivateFor(row));
        return behavior;
    }

    private static MockSaveHandler<TRow> CreateSaveHandlerFor<TRow>(MockDbConnection connection, bool isCreate, TRow row)
        where TRow : IRow, new()
    {
        return new MockSaveHandler<TRow>
        {
            Row = row,
            Old = isCreate ? null! : new TRow(),
            IsCreate = isCreate,
            IsUpdate = !isCreate,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenRowNotIdRow()
    {
        Assert.False(new CaptureLogBehavior().ActivateFor(new NotIdRow()));
    }

    [Fact]
    public void ActivateFor_ReturnsFalse_WhenNoCaptureLogAttribute()
    {
        Assert.False(new CaptureLogBehavior().ActivateFor(new NoAttrRow()));
    }

    [Fact]
    public void OnAudit_Delete_WithIsDeletedRow_SetsDeletedFlag()
    {
        var row = new DeletedAuditRow { Id = 1, Name = "X" };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new MockDeleteHandler<DeletedAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
    }

    [Fact]
    public void OnAudit_Delete_WithIsActiveDeletedRow_SetsInactive()
    {
        var row = new ActiveDeletedAuditRow { Id = 1, IsActive = 1 };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new MockDeleteHandler<ActiveDeletedAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
    }

    [Fact]
    public void OnAudit_Undelete_WithIsDeletedRow_LogsUpdate()
    {
        var row = new DeletedAuditRow { Id = 1, IsDeleted = true };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new FakeUndeleteHandler<DeletedAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
    }

    [Fact]
    public void OnAudit_Undelete_WithIsActiveDeletedRow_LogsUpdate()
    {
        var row = new ActiveDeletedAuditRow { Id = 1, IsActive = -1 };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new FakeUndeleteHandler<ActiveDeletedAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
    }

    [Fact]
    public void OnAudit_Undelete_WithDeleteLogRow_ClearsDeleteFields()
    {
        var row = new DeleteLogAuditRow { Id = 1, DeleteUserId = 5, DeleteDate = DateTime.Now };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new FakeUndeleteHandler<DeleteLogAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
    }

    [Fact]
    public async Task OnAuditAsync_Undelete_WithIsDeletedRow_LogsUpdate()
    {
        var row = new DeletedAuditRow { Id = 1, IsDeleted = true };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new FakeUndeleteHandler<DeletedAuditRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        await behavior.OnAuditAsync(handler, TestContext.Current.CancellationToken);

        Assert.Equal(2, connection.ManipulateRowCalls.Count);
        Assert.True(connection.ManipulateRowCalls[0].IsAsync);
    }

    [Fact]
    public void OnAudit_Undelete_WithPlainRow_ReturnsEarly()
    {
        var row = new MyRow { Id = 1, Name = "X" };
        var behavior = CreateBehaviorFor(row);
        using var connection = CreateConnection();
        var handler = new FakeUndeleteHandler<MyRow>
        {
            Row = row,
            Connection = connection,
            UnitOfWork = new MockUnitOfWork(connection),
            Context = Context()
        };

        behavior.OnAudit(handler);

        Assert.Equal(0, connection.AllCallCount);
    }

    [Fact]
    public void OnAudit_Update_WithOnlyAuditFieldsChanged_DoesNotLog()
    {
        using var connection = CreateConnection();
        var behavior = CreateBehaviorFor(new AuditRow());
        var handler = CreateSaveHandlerFor(connection, false, new AuditRow
        {
            Id = 1, Name = "Same", UpdateDate = new DateTime(2024, 1, 2)
        });
        handler.Old = new AuditRow { Id = 1, Name = "Same", UpdateDate = new DateTime(2024, 1, 1) };

        behavior.OnAudit(handler);

        Assert.Equal(0, connection.AllCallCount);
    }

    [Fact]
    public void OnAudit_Update_WithDataFieldChanged_Logs()
    {
        using var connection = CreateConnection();
        var behavior = CreateBehaviorFor(new AuditRow());
        var handler = CreateSaveHandlerFor(connection, false, new AuditRow { Id = 1, Name = "New" });
        handler.Old = new AuditRow { Id = 1, Name = "Old" };

        behavior.OnAudit(handler);

        Assert.True(connection.AllCallCount > 0);
    }

    [Fact]
    public void Log_Throws_WhenBothOldAndRowNull()
    {
        var behavior = CreateBehaviorFor(new MyRow { Id = 1 });
        var uow = new MockUnitOfWork(new MockDbConnection());
        Assert.Throws<ArgumentNullException>(() => behavior.Log(uow, null, null, null));
    }

    [Fact]
    public void OnAudit_Save_Throws_WhenLogRowNotCaptureLogRow()
    {
        using var connection = CreateConnection();
        var behavior = CreateBehaviorFor(new BadLogMasterRow { Id = 1 });
        var handler = CreateSaveHandlerFor(connection, true, new BadLogMasterRow { Id = 1 });

        Assert.Throws<InvalidOperationException>(() => behavior.OnAudit(handler));
    }

    [Fact]
    public void OnAudit_Save_Throws_WhenMappedIdFieldMissing()
    {
        using var connection = CreateConnection();
        var behavior = CreateBehaviorFor(new BadMappedMasterRow { Id = 1 });
        var handler = CreateSaveHandlerFor(connection, true, new BadMappedMasterRow { Id = 1 });

        Assert.Throws<InvalidOperationException>(() => behavior.OnAudit(handler));
    }

    [Fact]
    public void OnAudit_Save_Throws_WhenCapturedFieldHasNoMatch()
    {
        using var connection = CreateConnection();
        var behavior = CreateBehaviorFor(new MismatchMasterRow { Id = 1, Name = "X" });
        var handler = CreateSaveHandlerFor(connection, true, new MismatchMasterRow { Id = 1, Name = "X" });

        Assert.Throws<InvalidOperationException>(() => behavior.OnAudit(handler));
    }
}

