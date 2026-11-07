namespace Serenity.Services;

public class ListQueryCaptureExtensionsTests
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ListRow : Row<ListRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }

    private class CustomListRequest : ListRequest
    {
        public int? MasterId { get; set; }
    }

    private class CustomRequestHandler(IRequestContext context)
        : ListRequestHandler<ListRow, CustomListRequest>(context)
    {
    }

    private class DroppingHandler(IRequestContext context) : ListRequestHandler<ListRow>(context)
    {
        protected override ListRow ProcessEntity(ListRow row) => null!;

        protected override void OnReturn() { }
    }

    private class DroppingAsyncHandler(IRequestContext context) : ListRequestHandlerAsync<ListRow>(context)
    {
        protected override Task OnAfterExecuteQueryAsync(System.Threading.CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private class FilteringHandler(IRequestContext context) : ListRequestHandler<ListRow>(context)
    {
        protected override void ApplyFilters(SqlQuery query) => base.ApplyFilters(query);

        protected override void OnBeforeExecuteQuery() { }
    }

    private static DefaultRequestContext Context() => new(new NullBehaviorProvider(), new NullTwoLevelCache(),
        NullTextLocalizer.Instance, new MockPermissions(_ => true), new NullUserAccessor());

    [Fact]
    public void GetPostQueryOverrides_ListsTheOverridesRunningAfterTheQuery()
    {
        Assert.Equal(["OnReturn", "ProcessEntity"], new DroppingHandler(Context()).GetPostQueryOverrides());
        Assert.Equal(["OnAfterExecuteQueryAsync"], new DroppingAsyncHandler(Context()).GetPostQueryOverrides());
    }

    [Fact]
    public void GetPostQueryOverrides_IgnoresTheOverridesBeforeTheQuery_AndTheBaseClasses()
    {
        Assert.Empty(new FilteringHandler(Context()).GetPostQueryOverrides());
        Assert.Empty(new ListRequestHandler<ListRow>(Context()).GetPostQueryOverrides());
        Assert.Empty(new ListRequestHandlerAsync<ListRow>(Context()).GetPostQueryOverrides());
    }

    [Fact]
    public void GetPostQueryOverrides_LooksThroughTheWrappers()
    {
        var wrapper = new SyncToAsyncListRequestProcessorWrapper<ListRow>(new DroppingHandler(Context()));

        Assert.Equal(["OnReturn", "ProcessEntity"], wrapper.GetPostQueryOverrides());
    }

    [Fact]
    public void CreateRequest_CreatesTheRequestTypeOfTheWrappedHandler()
    {
        var handler = new CustomRequestHandler(Context());
        var wrapper = new SyncToAsyncListRequestProcessorWrapper<ListRow>(handler);

        Assert.IsType<CustomListRequest>(handler.CreateRequest());
        Assert.IsType<CustomListRequest>(((IListRequestHandler)wrapper).CreateRequest());
        Assert.Equal(typeof(CustomListRequest), wrapper.GetRequestType());
    }
}
