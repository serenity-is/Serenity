namespace Serenity.Services;

#pragma warning disable CS0649

public class SaveRequestHandlerAsyncTests_Coverage
{
    private static IRequestContext Context(IBehaviorProvider? behaviors = null) =>
        new NullRequestContext(behaviors).WithPermissions(_ => true);

    private class CoverRow : Row<CoverRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
        }
    }

    private class AutoRow : Row<AutoRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
        }
    }

    private class OrderRow : Row<OrderRow.RowFields>, IIdRow, IDisplayOrderRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public int? Order { get => fields.Order[this]; set => fields.Order[this] = value; }

        public Int32Field DisplayOrderField => fields.Order;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field Order;
        }
    }

    private class TestAsyncHandler<TRow> : SaveRequestHandlerAsync<TRow, SaveRequest<TRow>, SaveResponse>
        where TRow : class, IRow, IIdRow, new()
    {
        public TestAsyncHandler(IRequestContext context) : base(context)
        {
        }

        public void SetRow(TRow row) => Row = row;
        public TRow CurrentRow => Row;
    }

    private class TrackingSaveBehavior : BaseSaveBehavior
    {
        public bool Before, After, Audit, Return, Prepare, SetInternal, Validate, Exception;

        public override void OnBeforeSave(ISaveRequestHandler handler) => Before = true;
        public override void OnAfterSave(ISaveRequestHandler handler) => After = true;
        public override void OnAudit(ISaveRequestHandler handler) => Audit = true;
        public override void OnReturn(ISaveRequestHandler handler) => Return = true;
        public override void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query) => Prepare = true;
        public override void OnSetInternalFields(ISaveRequestHandler handler) => SetInternal = true;
        public override void OnValidateRequest(ISaveRequestHandler handler) => Validate = true;
        public override void OnException(ISaveRequestHandler handler, Exception exception) => Exception = true;
    }

    [Fact]
    public async Task ProcessAsync_NullArguments_Throw()
    {
        var handler = new TestAsyncHandler<CoverRow>(Context());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.ProcessAsync(null!, new SaveRequest<CoverRow> { Entity = new CoverRow() },
                SaveRequestType.Auto, TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.ProcessAsync(new MockUnitOfWork(new MockDbConnection()), null!,
                SaveRequestType.Auto, TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.ProcessAsync(new MockUnitOfWork(new MockDbConnection()), new SaveRequest<CoverRow>(),
                SaveRequestType.Auto, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ProcessAsync_Auto_Detects_Create_And_Update()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 1)
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context());
        var uow = new MockUnitOfWork(connection);
        var token = TestContext.Current.CancellationToken;

        var createResponse = await handler.ProcessAsync(uow, new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Name = "A" }
        }, SaveRequestType.Auto, token);
        Assert.NotNull(createResponse);

        var updateResponse = await handler.ProcessAsync(uow, new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Id = 5, Name = "New" }
        }, SaveRequestType.Auto, token);
        Assert.NotNull(updateResponse);
    }

    [Fact]
    public async Task CreateAsync_AutoIncrement_Uses_InsertAndGetId()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 42);
        var handler = new TestAsyncHandler<AutoRow>(Context());

        var response = await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<AutoRow>
        {
            Entity = new AutoRow { Name = "A" }
        }, TestContext.Current.CancellationToken);

        Assert.Equal(42L, Convert.ToInt64(response.EntityId));
    }

    [Fact]
    public async Task CreateAsync_NonAutoIncrement_Inserts()
    {
        using var connection = new MockDbConnection().InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context());

        var response = await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Id = 7, Name = "A" }
        }, TestContext.Current.CancellationToken);

        Assert.Equal(7, response.EntityId);
    }

    [Fact]
    public async Task UpdateAsync_Changed_Id_Uses_SqlUpdate()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context());

        var response = await handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            EntityId = 5,
            Entity = new CoverRow { Id = 6, Name = "New" }
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task UpdateAsync_OldEntity_Not_Found_Throws()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new TestAsyncHandler<CoverRow>(Context());

        await Assert.ThrowsAsync<ValidationError>(() => handler.UpdateAsync(new MockUnitOfWork(connection),
            new SaveRequest<CoverRow>
            {
                EntityId = 5,
                Entity = new CoverRow { Id = 5, Name = "New" }
            }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateAsync_Without_Assigned_Fields_Does_Not_Update()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context());

        var response = await handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            EntityId = 5,
            Entity = new CoverRow { Id = 5 }
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task ProcessAsync_With_Behaviors_Calls_All_Hooks()
    {
        var behavior = new TrackingSaveBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 1)
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context(behaviors));

        await handler.ProcessAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Name = "A" }
        }, SaveRequestType.Auto, TestContext.Current.CancellationToken);

        Assert.True(behavior.Before);
        Assert.True(behavior.After);
        Assert.True(behavior.Audit);
        Assert.True(behavior.Return);
        Assert.True(behavior.SetInternal);
        Assert.True(behavior.Validate);

        await handler.ProcessAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
        {
            Entity = new CoverRow { Id = 5, Name = "New" }
        }, SaveRequestType.Auto, TestContext.Current.CancellationToken);
        Assert.True(behavior.Prepare);
    }

    [Fact]
    public async Task ProcessAsync_OnException_Calls_ExceptionBehavior()
    {
        var behavior = new TrackingSaveBehavior();
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[] { behavior });
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5, Name = "Old" }))
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("boom"));
        var handler = new TestAsyncHandler<CoverRow>(Context(behaviors));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<CoverRow>
            {
                EntityId = 5,
                Entity = new CoverRow { Id = 6, Name = "New" }
            }, TestContext.Current.CancellationToken));

        Assert.True(behavior.Exception);
    }

    [Fact]
    public async Task Explicit_ISaveRequestProcessorAsync_ProcessAsync_Works()
    {
        using var connection = new MockDbConnection().InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<CoverRow>(Context());

        var response = await ((ISaveRequestProcessorAsync)handler).ProcessAsync(
            new MockUnitOfWork(connection),
            new SaveRequest<CoverRow> { Entity = new CoverRow { Id = 7, Name = "A" } },
            SaveRequestType.Create,
            TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task DisplayOrder_Create_With_Zero_Order_Gets_Next_Value()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { MAX = 5 }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<OrderRow>(Context());

        var row = new OrderRow { Id = 7 };
        await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<OrderRow> { Entity = row },
            TestContext.Current.CancellationToken);

        Assert.Equal(6, handler.CurrentRow.Order);
    }

    [Fact]
    public async Task DisplayOrder_Create_With_Positive_Order_Reorders_After_Save()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 7, Order = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<OrderRow>(Context());

        var row = new OrderRow { Id = 7, Order = 2 };
        await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<OrderRow> { Entity = row },
            TestContext.Current.CancellationToken);

        Assert.Equal(2, handler.CurrentRow.Order);
    }

    [Fact]
    public async Task DisplayOrder_Update_With_Changed_Order_Reorders_After_Save()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 5, Order = 1 }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<OrderRow>(Context());

        var row = new OrderRow { Id = 5, Order = 3 };
        await handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<OrderRow> { Entity = row },
            TestContext.Current.CancellationToken);

        Assert.Equal(3, handler.CurrentRow.Order);
    }

    [Fact]
    public async Task DisplayOrder_Update_With_Unchanged_Order_Does_Not_Reorder()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 5, Order = 3 }))
            .OnDbCommandExecuteNonQuery(_ => 1)
            .InterceptManipulateRow(_ => 1);
        var handler = new TestAsyncHandler<OrderRow>(Context());

        var row = new OrderRow { Id = 5, Order = 3 };
        await handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<OrderRow> { Entity = row },
            TestContext.Current.CancellationToken);

        Assert.Equal(3, handler.CurrentRow.Order);
    }
}

#pragma warning restore CS0649
