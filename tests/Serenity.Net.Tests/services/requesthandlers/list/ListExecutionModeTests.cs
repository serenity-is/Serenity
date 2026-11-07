namespace Serenity.Services;

public class ListExecutionModeTests
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ListRow : Row<ListRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [QuickSearch]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Name = null!;
        }
    }

    /// <summary>
    /// Filters by a value it reads from the database, and records the mode in each step.
    /// </summary>
    private class CaptureAwareBehavior : IListBehaviorSync
    {
        public List<string> Steps { get; } = [];

        private void Record(IListRequestHandler handler, string step) => Steps.Add(step + ":" + handler.ExecutionMode);

        public void OnValidateRequest(IListRequestHandler handler) => Record(handler, "Validate");

        public void OnPrepareQuery(IListRequestHandler handler, SqlQuery query) => Record(handler, "Prepare");

        public void OnApplyFilters(IListRequestHandler handler, SqlQuery query)
        {
            Record(handler, "Filters");
            var minId = SqlHelper.ExecuteScalar(handler.Connection, "SELECT MinId FROM Settings");
            query.Where(new Criteria("T0.Id") > Convert.ToInt32(minId));
        }

        public void OnBeforeExecuteQuery(IListRequestHandler handler) => Record(handler, "BeforeExecute");
        public void OnAfterExecuteQuery(IListRequestHandler handler) => Record(handler, "AfterExecute");
        public void OnReturn(IListRequestHandler handler) => Record(handler, "Return");
    }

    private class TestListHandler(IRequestContext context, IListBehavior behavior) : ListRequestHandler<ListRow>(context)
    {
        protected override IEnumerable<IListBehavior> GetBehaviors() => [behavior];
    }

    private class TestListHandlerAsync(IRequestContext context, IListBehavior behavior) : ListRequestHandlerAsync<ListRow>(context)
    {
        protected override IEnumerable<IListBehavior> GetBehaviors() => [behavior];
    }

    private static DefaultRequestContext Context() => new(new NullBehaviorProvider(), new NullTwoLevelCache(),
        NullTextLocalizer.Instance, new MockPermissions(_ => true), new NullUserAccessor(),
        new RequestHandlerSettings { MaxPageSize = 10 });

    private class RecordingConnection
    {
        public List<string> Readers { get; } = [];
        public List<string> Scalars { get; } = [];

        public MockDbConnection Create() => new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Readers.Add(args.CommandText);
                return args.ToMockReader(new { Id = 5, Name = "A" });
            })
            .InterceptExecuteScalar(args =>
            {
                Scalars.Add(args.CommandText);
                return 3;
            });
    }

    private static ListRequest Request() => new()
    {
        Take = 5,
        Skip = 10,
        Sort = [new SortBy("Name")],
        ContainsText = "x"
    };

    private static void AssertCapturedQuery(SqlQuery query, RecordingConnection recording, CaptureAwareBehavior behavior)
    {
        var sql = query.ToString();

        Assert.Contains("T0.Id > @p", sql);
        Assert.Contains("T0.[Name] LIKE @p", sql);
        Assert.DoesNotContain("ORDER BY", sql);
        Assert.DoesNotContain("OFFSET", sql);
        Assert.DoesNotContain("COUNT(*)", sql);
        Assert.Equal(0, query.Skip());
        Assert.Equal(0, query.Take());
        Assert.Contains(3, query.Params!.Values);
        // the behavior's own query runs, the list query does not
        Assert.Single(recording.Scalars);
        Assert.Empty(recording.Readers);
        Assert.Equal(["Validate:QueryCapture", "Prepare:QueryCapture", "Filters:QueryCapture",
            "BeforeExecute:QueryCapture"], behavior.Steps);
    }

    [Fact]
    public void ExecutionMode_IsNormal_ByDefault()
    {
        IListRequestHandler handler = new ListRequestHandler<ListRow>(Context());

        Assert.Equal(ListExecutionMode.Normal, handler.ExecutionMode);
    }

    [Fact]
    public void CaptureQuery_BuildsTheFilteredQuery_WithoutExecutingIt()
    {
        var behavior = new CaptureAwareBehavior();
        var handler = new TestListHandler(Context(), behavior);
        var recording = new RecordingConnection();
        using var connection = recording.Create();

        var query = handler.CaptureQuery(connection, Request());

        AssertCapturedQuery(query, recording, behavior);
        Assert.Equal(ListExecutionMode.Normal, handler.ExecutionMode);
    }

    [Fact]
    public async Task CaptureQueryAsync_BuildsTheFilteredQuery_WithoutExecutingIt()
    {
        var behavior = new CaptureAwareBehavior();
        var handler = new TestListHandlerAsync(Context(), behavior);
        var recording = new RecordingConnection();
        using var connection = recording.Create();

        var query = await handler.CaptureQueryAsync(connection, Request(), TestContext.Current.CancellationToken);

        AssertCapturedQuery(query, recording, behavior);
        Assert.Equal(ListExecutionMode.Normal, handler.ExecutionMode);
    }

    [Fact]
    public void Process_AfterCaptureQuery_ExecutesNormally()
    {
        var behavior = new CaptureAwareBehavior();
        var handler = new TestListHandler(Context(), behavior);
        var recording = new RecordingConnection();
        using var connection = recording.Create();

        handler.CaptureQuery(connection, Request());
        behavior.Steps.Clear();
        var response = handler.List(connection, new ListRequest());

        Assert.Single(response.Entities);
        Assert.Single(recording.Readers);
        Assert.Contains("ORDER BY", handler.Query.ToString());
        Assert.Equal(["Validate:Normal", "Prepare:Normal", "Filters:Normal", "BeforeExecute:Normal",
            "AfterExecute:Normal", "Return:Normal"], behavior.Steps);
    }

    [Fact]
    public async Task Wrappers_CaptureTheQueryOfTheWrappedHandler()
    {
        var recording = new RecordingConnection();
        using var connection = recording.Create();

        IListRequestProcessorAsync syncToAsync = new SyncToAsyncListRequestProcessorWrapper<ListRow>(
            new TestListHandler(Context(), new CaptureAwareBehavior()));
        IListRequestProcessor asyncToSync = new AsyncToSyncListRequestProcessorWrapper<ListRow>(
            new TestListHandlerAsync(Context(), new CaptureAwareBehavior()));

        Assert.Contains("T0.Id > @p",
            (await syncToAsync.CaptureQueryAsync(connection, new ListRequest(), TestContext.Current.CancellationToken)).ToString());
        Assert.Contains("T0.Id > @p", asyncToSync.CaptureQuery(connection, new ListRequest()).ToString());
        Assert.Empty(recording.Readers);
    }

    [Fact]
    public async Task CaptureQuery_IsNotSupported_ByHandlersThatDoNotImplementIt()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<NotSupportedException>(() =>
            ((IListRequestProcessor)new MockListHandler<ListRow>()).CaptureQuery(connection, new ListRequest()));
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            ((IListRequestProcessorAsync)new MockListHandlerAsync<ListRow>()).CaptureQueryAsync(connection, new ListRequest(),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Wrappers_ForwardTheExecutionMode()
    {
        var sync = new MockListHandler<ListRow> { ExecutionMode = ListExecutionMode.QueryCapture };
        var async = new MockListHandlerAsync<ListRow> { ExecutionMode = ListExecutionMode.QueryCapture };

        Assert.Equal(ListExecutionMode.QueryCapture,
            ((IListRequestHandler)new SyncToAsyncListRequestProcessorWrapper<ListRow>(sync)).ExecutionMode);
        Assert.Equal(ListExecutionMode.QueryCapture,
            ((IListRequestHandler)new AsyncToSyncListRequestProcessorWrapper<ListRow>(async)).ExecutionMode);
    }
}
