namespace Serenity.Services;

#pragma warning disable CS0649

public class UndeleteRequestHandlerTests_Coverage
{
    private static IRequestContext Context(IBehaviorProvider? behaviors = null,
        Func<string, bool>? hasPermission = null) =>
        new NullRequestContext(behaviors).WithPermissions(hasPermission ?? (_ => true));

    [TableName("UndelSoft")]
    private class SoftRow : Row<SoftRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public BooleanField IsDeleted;
        }
    }

    [TableName("UndelActive")]
    private class ActiveRow : Row<ActiveRow.RowFields>, IIdRow, IIsActiveDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }
        public Int16Field IsActiveField => fields.IsActive;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int16Field IsActive;
        }
    }

    [TableName("UndelLog")]
    private class LogRow : Row<LogRow.RowFields>, IIdRow, IDeleteLogRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public DateTime? DeleteDate { get => fields.DeleteDate[this]; set => fields.DeleteDate[this] = value; }
        public long? DeleteUserId { get => fields.DeleteUserId[this]; set => fields.DeleteUserId[this] = value; }
        public DateTimeField DeleteDateField => fields.DeleteDate;
        public Field DeleteUserIdField => fields.DeleteUserId;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public DateTimeField DeleteDate;
            public Int64Field DeleteUserId;
        }
    }

    [TableName("UndelLogSoft")]
    private class LogSoftRow : Row<LogSoftRow.RowFields>, IIdRow, IIsDeletedRow, IDeleteLogRow
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
            public Int32Field Id;
            public BooleanField IsDeleted;
            public DateTimeField DeleteDate;
            public Int64Field DeleteUserId;
        }
    }

    [TableName("UndelNone")]
    private class NoMarkerRow : Row<NoMarkerRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
        }
    }

    [TableName("UndelOrder")]
    private class OrderRow : Row<OrderRow.RowFields>, IIdRow, IIsDeletedRow, IDisplayOrderRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public int? Order { get => fields.Order[this]; set => fields.Order[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;
        public Int32Field DisplayOrderField => fields.Order;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public BooleanField IsDeleted;
            public Int32Field Order;
        }
    }

    [DeletePermission("Test.UD")]
    private class DeletePermRow : Row<DeletePermRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public BooleanField IsDeleted;
        }
    }

    [ModifyPermission("Test.UM")]
    private class ModifyPermRow : Row<ModifyPermRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public BooleanField IsDeleted;
        }
    }

    [ReadPermission("Test.UR")]
    private class ReadPermRow : Row<ReadPermRow.RowFields>, IIdRow, IIsDeletedRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public bool? IsDeleted { get => fields.IsDeleted[this]; set => fields.IsDeleted[this] = value; }
        public BooleanField IsDeletedField => fields.IsDeleted;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public BooleanField IsDeleted;
        }
    }

    private class SyncHandler<TRow> : UndeleteRequestHandler<TRow, UndeleteRequest, UndeleteResponse>
        where TRow : class, IRow, IIdRow, new()
    {
        public SyncHandler(IRequestContext context) : base(context)
        {
        }
    }

    private class AsyncHandler<TRow> : UndeleteRequestHandlerAsync<TRow, UndeleteRequest, UndeleteResponse>
        where TRow : class, IRow, IIdRow, new()
    {
        public AsyncHandler(IRequestContext context) : base(context)
        {
        }
    }

    private class TrackingBehavior : BaseUndeleteBehavior
    {
        public bool Prepare, Validate, Before, After, Audit, Return, Exception;

        public override void OnPrepareQuery(IUndeleteRequestHandler handler, SqlQuery query) => Prepare = true;
        public override void OnValidateRequest(IUndeleteRequestHandler handler) => Validate = true;
        public override void OnBeforeUndelete(IUndeleteRequestHandler handler) => Before = true;
        public override void OnAfterUndelete(IUndeleteRequestHandler handler) => After = true;
        public override void OnAudit(IUndeleteRequestHandler handler) => Audit = true;
        public override void OnReturn(IUndeleteRequestHandler handler) => Return = true;
        public override void OnException(IUndeleteRequestHandler handler, Exception exception) => Exception = true;
    }

    private static MockDbConnection Connection(object? row = null,
        Func<int>? nonQuery = null)
    {
        var connection = new MockDbConnection()
            .InterceptExecuteReader(args => row == null ? new MockDbDataReader() : args.ToMockReader(row))
            .InterceptExecuteNonQuery(_ => (nonQuery ?? (() => 1))())
            .OnDbCommandExecuteNonQuery(_ => (nonQuery ?? (() => 1))())
            .InterceptManipulateRow(_ => 1);
        return connection;
    }

    private static IBehaviorProvider Behaviors(BaseUndeleteBehavior behavior) =>
        new MockBehaviorProvider((_, _, _) => new object[] { behavior });

    // ---- base / sync ----

    [Fact]
    public void Constructor_NullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SyncHandler<SoftRow>(null!));
    }

    [Fact]
    public void Uninitialized_Properties_Throw()
    {
        var handler = new SyncHandler<SoftRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
        Assert.Throws<InvalidOperationException>(() => handler.Response);
        Assert.Throws<InvalidOperationException>(() => handler.Connection);
        Assert.Throws<InvalidOperationException>(() => handler.UnitOfWork);
    }

    [Fact]
    public void Undelete_NullUnitOfWork_Throws()
    {
        var handler = new SyncHandler<SoftRow>(Context());
        Assert.Throws<ArgumentNullException>(() => handler.Undelete(null!, new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_NullEntityId_Throws()
    {
        var handler = new SyncHandler<SoftRow>(Context());
        Assert.Throws<ValidationError>(() => handler.Undelete(new MockUnitOfWork(Connection()), new UndeleteRequest()));
    }

    [Fact]
    public void Undelete_EntityNotFound_Throws()
    {
        var handler = new SyncHandler<SoftRow>(Context());
        Assert.Throws<ValidationError>(() => handler.Undelete(new MockUnitOfWork(Connection()),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_SoftDeleted_Restores()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<SoftRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_SoftNotDeleted_Returns_WasNotDeleted()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = false });
        var handler = new SyncHandler<SoftRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.True(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_ActiveDeleted_Restores()
    {
        using var connection = Connection(new { Id = 5, IsActive = (short)-1 });
        var handler = new SyncHandler<ActiveRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_ActiveNotDeleted_Returns_WasNotDeleted()
    {
        using var connection = Connection(new { Id = 5, IsActive = (short)1 });
        var handler = new SyncHandler<ActiveRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.True(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_DeleteLog_Restores()
    {
        using var connection = Connection(new
        {
            Id = 5,
            DeleteUserId = (long?)9,
            DeleteDate = (DateTime?)DateTime.Now
        });
        var handler = new SyncHandler<LogRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_DeleteLogNotDeleted_Returns_WasNotDeleted()
    {
        using var connection = Connection(new
        {
            Id = 5,
            DeleteUserId = (long?)null,
            DeleteDate = (DateTime?)null
        });
        var handler = new SyncHandler<LogRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.True(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_LogAndSoft_Uses_SoftBranch()
    {
        using var connection = Connection(new
        {
            Id = 5,
            IsDeleted = true,
            DeleteUserId = (long?)9,
            DeleteDate = (DateTime?)DateTime.Now
        });
        var handler = new SyncHandler<LogSoftRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_Without_Marker_Throws_NotImplemented()
    {
        using var connection = Connection(new { Id = 5 });
        var handler = new SyncHandler<NoMarkerRow>(Context());

        Assert.Throws<NotImplementedException>(() => handler.Undelete(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_ExecuteReturnsZero_Throws()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true }, () => 0);
        var handler = new SyncHandler<SoftRow>(Context());

        Assert.Throws<ValidationError>(() => handler.Undelete(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_With_Behaviors_Calls_All_Hooks()
    {
        var behavior = new TrackingBehavior();
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<SoftRow>(Context(Behaviors(behavior)));

        handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.True(behavior.Prepare);
        Assert.True(behavior.Validate);
        Assert.True(behavior.Before);
        Assert.True(behavior.After);
        Assert.True(behavior.Audit);
        Assert.True(behavior.Return);
    }

    [Fact]
    public void Undelete_OnException_Calls_ExceptionBehavior()
    {
        var behavior = new TrackingBehavior();
        using var connection = Connection(new { Id = 5, IsDeleted = true }, () => throw new InvalidOperationException("boom"));
        var handler = new SyncHandler<SoftRow>(Context(Behaviors(behavior)));

        Assert.Throws<InvalidOperationException>(() => handler.Undelete(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }));

        Assert.True(behavior.Exception);
    }

    [Fact]
    public void Undelete_DisplayOrder_Reorders()
    {
        using var connection = Connection(new { Id = 5, Order = 3, IsDeleted = true });
        var handler = new SyncHandler<OrderRow>(Context());

        var response = handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public void Undelete_DeletePermission_Validates()
    {
        using var allowed = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<DeletePermRow>(Context(hasPermission: _ => true));
        Assert.NotNull(handler.Undelete(new MockUnitOfWork(allowed), new UndeleteRequest { EntityId = 5 }));

        using var denied = Connection(new { Id = 5, IsDeleted = true });
        var deniedHandler = new SyncHandler<DeletePermRow>(Context(hasPermission: _ => false));
        Assert.Throws<ValidationError>(() => deniedHandler.Undelete(new MockUnitOfWork(denied),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_ModifyPermission_Validates()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<ModifyPermRow>(Context(hasPermission: _ => true));
        Assert.NotNull(handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 }));

        using var denied = Connection(new { Id = 5, IsDeleted = true });
        var deniedHandler = new SyncHandler<ModifyPermRow>(Context(hasPermission: _ => false));
        Assert.Throws<ValidationError>(() => deniedHandler.Undelete(new MockUnitOfWork(denied),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Undelete_ReadPermission_Validates()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<ReadPermRow>(Context(hasPermission: _ => true));
        Assert.NotNull(handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 }));

        using var denied = Connection(new { Id = 5, IsDeleted = true });
        var deniedHandler = new SyncHandler<ReadPermRow>(Context(hasPermission: _ => false));
        Assert.Throws<ValidationError>(() => deniedHandler.Undelete(new MockUnitOfWork(denied),
            new UndeleteRequest { EntityId = 5 }));
    }

    [Fact]
    public void Explicit_IUndeleteRequestProcessor_Process_Works()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<SoftRow>(Context());

        var response = ((IUndeleteRequestProcessor)handler).Process(
            new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        Assert.NotNull(response);
    }

    [Fact]
    public void Explicit_IUndeleteRequestHandler_Members_Forward()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new SyncHandler<SoftRow>(Context());
        handler.Undelete(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 });

        IUndeleteRequestHandler iface = handler;
        Assert.Same(handler.Row, iface.Row);
        Assert.Same(handler.Request, iface.Request);
        Assert.Same(handler.Response, iface.Response);
    }

    // ---- async ----

    [Fact]
    public async Task UndeleteAsync_NullArguments_Throw()
    {
        var handler = new AsyncHandler<SoftRow>(Context());
        var token = TestContext.Current.CancellationToken;

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.UndeleteAsync(null!, new UndeleteRequest { EntityId = 5 }, token));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.UndeleteAsync(new MockUnitOfWork(Connection()), null!, token));
    }

    [Fact]
    public async Task UndeleteAsync_NullEntityId_Throws()
    {
        var handler = new AsyncHandler<SoftRow>(Context());
        await Assert.ThrowsAsync<ValidationError>(() => handler.UndeleteAsync(
            new MockUnitOfWork(Connection()), new UndeleteRequest(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UndeleteAsync_EntityNotFound_Throws()
    {
        var handler = new AsyncHandler<SoftRow>(Context());
        await Assert.ThrowsAsync<ValidationError>(() => handler.UndeleteAsync(
            new MockUnitOfWork(Connection()), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UndeleteAsync_SoftDeleted_Restores()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new AsyncHandler<SoftRow>(Context());

        var response = await handler.UndeleteAsync(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public async Task UndeleteAsync_WasNotDeleted()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = false });
        var handler = new AsyncHandler<SoftRow>(Context());

        var response = await handler.UndeleteAsync(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.True(response.WasNotDeleted);
    }

    [Fact]
    public async Task UndeleteAsync_Active_Restores()
    {
        using var connection = Connection(new { Id = 5, IsActive = (short)-1 });
        var handler = new AsyncHandler<ActiveRow>(Context());

        var response = await handler.UndeleteAsync(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public async Task UndeleteAsync_DeleteLog_Restores()
    {
        using var connection = Connection(new
        {
            Id = 5,
            DeleteUserId = (long?)9,
            DeleteDate = (DateTime?)DateTime.Now
        });
        var handler = new AsyncHandler<LogRow>(Context());

        var response = await handler.UndeleteAsync(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public async Task UndeleteAsync_Without_Marker_Throws()
    {
        using var connection = Connection(new { Id = 5 });
        var handler = new AsyncHandler<NoMarkerRow>(Context());

        await Assert.ThrowsAsync<NotImplementedException>(() => handler.UndeleteAsync(
            new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UndeleteAsync_ExecuteReturnsZero_Throws()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true }, () => 0);
        var handler = new AsyncHandler<SoftRow>(Context());

        await Assert.ThrowsAsync<ValidationError>(() => handler.UndeleteAsync(
            new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UndeleteAsync_With_Behaviors_Calls_All_Hooks()
    {
        var behavior = new TrackingBehavior();
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new AsyncHandler<SoftRow>(Context(Behaviors(behavior)));

        await handler.UndeleteAsync(new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken);

        Assert.True(behavior.Prepare);
        Assert.True(behavior.Validate);
        Assert.True(behavior.Before);
        Assert.True(behavior.After);
        Assert.True(behavior.Audit);
        Assert.True(behavior.Return);
    }

    [Fact]
    public async Task UndeleteAsync_OnException_Calls_ExceptionBehavior()
    {
        var behavior = new TrackingBehavior();
        using var connection = Connection(new { Id = 5, IsDeleted = true }, () => throw new InvalidOperationException("boom"));
        var handler = new AsyncHandler<SoftRow>(Context(Behaviors(behavior)));

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.UndeleteAsync(
            new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken));

        Assert.True(behavior.Exception);
    }

    [Fact]
    public async Task UndeleteAsync_DisplayOrder_Reorders()
    {
        using var connection = Connection(new { Id = 5, Order = 3, IsDeleted = true });
        var handler = new AsyncHandler<OrderRow>(Context());

        var response = await handler.UndeleteAsync(new MockUnitOfWork(connection),
            new UndeleteRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.False(response.WasNotDeleted);
    }

    [Fact]
    public async Task Explicit_IUndeleteRequestProcessorAsync_ProcessAsync_Works()
    {
        using var connection = Connection(new { Id = 5, IsDeleted = true });
        var handler = new AsyncHandler<SoftRow>(Context());

        var response = await ((IUndeleteRequestProcessorAsync)handler).ProcessAsync(
            new MockUnitOfWork(connection), new UndeleteRequest { EntityId = 5 },
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }
}

#pragma warning restore CS0649
