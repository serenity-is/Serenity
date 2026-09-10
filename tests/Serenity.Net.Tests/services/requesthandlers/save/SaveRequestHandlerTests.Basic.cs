using System.Threading;

namespace Serenity.Services;

public class SaveRequestHandlerTests_Basic
{
    private static IRequestContext Context() => new NullRequestContext().WithPermissions(_ => true);

    [Fact]
    public void Create_InsertsRow()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 1);
        var handler = new SaveRequestHandler<IdNameRow>(Context());

        var response = handler.Create(new MockUnitOfWork(connection), new SaveRequest<IdNameRow>
        {
            Entity = new IdNameRow { Name = "Test" }
        });

        Assert.NotNull(response);
        Assert.Single(connection.ManipulateRowCalls);
    }

    [Fact]
    public async Task CreateAsync_InsertsRow()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(_ => 1);
        var handler = new SaveRequestHandlerAsync<IdNameRow>(Context());

        var response = await handler.CreateAsync(new MockUnitOfWork(connection), new SaveRequest<IdNameRow>
        {
            Entity = new IdNameRow { Name = "Test" }
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Single(connection.ManipulateRowCalls);
    }

    [Fact]
    public void Update_UpdatesRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 5, Name = "Old" })).InterceptManipulateRow(_ => 1);
        var handler = new SaveRequestHandler<IdNameRow>(Context());

        var response = handler.Update(new MockUnitOfWork(connection), new SaveRequest<IdNameRow>
        {
            Entity = new IdNameRow { ID = 5, Name = "New" }
        });

        Assert.NotNull(response);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesRow()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { ID = 5, Name = "Old" })).InterceptManipulateRow(_ => 1);
        var handler = new SaveRequestHandlerAsync<IdNameRow>(Context());

        var response = await handler.UpdateAsync(new MockUnitOfWork(connection), new SaveRequest<IdNameRow>
        {
            Entity = new IdNameRow { ID = 5, Name = "New" }
        }, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
    }

    [Fact]
    public void UninitializedProperties_Throw()
    {
        var handler = new SaveRequestHandler<IdNameRow>(Context());
        Assert.Throws<InvalidOperationException>(() => handler.Row);
        Assert.Throws<InvalidOperationException>(() => handler.Request);
        Assert.Throws<InvalidOperationException>(() => handler.Response);
        Assert.Throws<InvalidOperationException>(() => handler.Connection);
        Assert.Throws<InvalidOperationException>(() => handler.UnitOfWork);
    }
}

