namespace Serenity.Services;

public class ListRequestHandlerTests_Basic
{
    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public void List_ReturnsRows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 1, Name = "A" }))
            .InterceptExecuteNonQuery(_ => 1);
        var handler = new ListRequestHandler<IdNameRow>(Context());

        var response = handler.List(connection, new ListRequest());

        Assert.NotNull(response);
        Assert.NotNull(response.Entities);
    }

    [Fact]
    public void List_ReturnsEmpty()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());
        var handler = new ListRequestHandler<IdNameRow>(Context());

        var response = handler.List(connection, new ListRequest());

        Assert.NotNull(response);
        Assert.Empty(response.Entities);
    }

    [Fact]
    public void List_UninitializedProperties_Throw()
    {
        var handler = new ListRequestHandler<IdNameRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
    }
}
