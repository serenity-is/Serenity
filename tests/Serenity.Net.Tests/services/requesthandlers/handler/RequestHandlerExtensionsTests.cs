namespace Serenity.Services;

public class RequestHandlerExtensionsTests
{
    private class TestRow : Row<TestRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty, Identity]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty, Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
        }
    }

    private class PlainHandler : IRequestHandler
    {
    }

    private class CustomRequest
    {
        public string Value { get; set; }
    }

    private class CustomRequestTypeHandler : IRequestHandler, IRequestType<CustomRequest>
    {
    }

    [Fact]
    public void CreateRequest_List_Creates_ListRequest()
    {
        var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
        Assert.IsType<ListRequest>(handler.CreateRequest());
    }

    [Fact]
    public void CreateRequest_Retrieve_Creates_RetrieveRequest()
    {
        var handler = new RetrieveRequestHandler<TestRow>(new NullRequestContext());
        Assert.IsType<RetrieveRequest>(handler.CreateRequest());
    }

    [Fact]
    public void CreateRequest_Delete_Creates_DeleteRequest()
    {
        var handler = new DeleteRequestHandler<TestRow>(new NullRequestContext());
        Assert.IsType<DeleteRequest>(handler.CreateRequest());
    }

    [Fact]
    public void CreateRequest_Undelete_Creates_UndeleteRequest()
    {
        var handler = new UndeleteRequestHandler<TestRow>(new NullRequestContext());
        Assert.IsType<UndeleteRequest>(handler.CreateRequest());
    }

    [Fact]
    public void CreateRequest_Save_Generic_Creates_SaveRequest()
    {
        var handler = new SaveRequestHandler<TestRow>(new NullRequestContext());
        var request = handler.CreateRequest<TestRow>();
        Assert.IsType<SaveRequest<TestRow>>(request);
    }

    [Fact]
    public void CreateRequest_Save_NonGeneric_Creates_SaveRequest()
    {
        var handler = new SaveRequestHandler<TestRow>(new NullRequestContext());
        ISaveRequest request = handler.CreateRequest();
        Assert.IsType<SaveRequest<TestRow>>(request);
    }

    [Fact]
    public void GetRequestType_Returns_Type_For_List_Handler()
    {
        var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
        Assert.Equal(typeof(ListRequest), handler.GetRequestType());
    }

    [Fact]
    public void GetRequestType_Returns_Null_When_Not_Declared()
    {
        Assert.Null(new PlainHandler().GetRequestType());
    }

    [Fact]
    public void GetResponseType_Returns_Type_For_List_Handler()
    {
        var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
        Assert.Equal(typeof(ListResponse<TestRow>), handler.GetResponseType());
    }

    [Fact]
    public void GetResponseType_Returns_Null_When_Not_Declared()
    {
        Assert.Null(new PlainHandler().GetResponseType());
    }

    [Fact]
    public void GetRequestType_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => RequestHandlerExtensions.GetRequestType(null));
    }

    [Fact]
    public void GetResponseType_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => RequestHandlerExtensions.GetResponseType(null));
    }

    [Fact]
    public void CreateRequest_Throws_For_Null_Handler()
    {
        Assert.Throws<ArgumentNullException>(() =>
            RequestHandlerExtensions.CreateRequest((IListRequestHandler)null));
    }

    [Fact]
    public void InstantiateRequest_Throws_For_Null_Handler()
    {
        Assert.Throws<ArgumentNullException>(() =>
            RequestHandlerExtensions.InstantiateRequest<ListRequest>(null));
    }

    [Fact]
    public void InstantiateRequest_Throws_When_Request_Type_Is_Not_Declared()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RequestHandlerExtensions.InstantiateRequest<ListRequest>(new PlainHandler()));

        Assert.Contains(nameof(PlainHandler), ex.Message);
    }

    [Fact]
    public void InstantiateRequest_Throws_When_Request_Type_Is_Not_Assignable()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RequestHandlerExtensions.InstantiateRequest<ListRequest>(new CustomRequestTypeHandler()));

        Assert.Contains(nameof(CustomRequest), ex.Message);
    }
}
