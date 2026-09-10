namespace Serenity.Services;

public class RetrieveRequestHandlerTests_Basic
{
    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public void Retrieve_ReturnsRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 5, Name = "A" }));
        var handler = new RetrieveRequestHandler<IdNameRow>(Context());

        var response = handler.Retrieve(connection, new RetrieveRequest { EntityId = 5 });

        Assert.NotNull(response);
        Assert.NotNull(response.Entity);
        Assert.Equal(5, response.Entity.ID);
    }

    [Fact]
    public void Retrieve_UninitializedProperties_Throw()
    {
        var handler = new RetrieveRequestHandler<IdNameRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
        Assert.Throws<InvalidOperationException>(() => handler.Response);
    }

    [Fact]
    public async Task RetrieveAsync_ReturnsRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 5, Name = "A" }));
        var handler = new RetrieveRequestHandlerAsync<IdNameRow>(Context());

        var response = await handler.RetrieveAsync(connection,
            new RetrieveRequest { EntityId = 5 }, TestContext.Current.CancellationToken);

        Assert.NotNull(response.Entity);
    }
}
