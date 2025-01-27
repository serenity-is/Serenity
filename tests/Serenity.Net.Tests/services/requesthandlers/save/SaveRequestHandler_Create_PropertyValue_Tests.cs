namespace Serenity.Services;

public partial class SaveRequestHandler_Create_PropertyValue_Tests
{
    [Fact]
    public void Public_PropertyValues_AreAsExpected()
    {
        var context = new NullRequestContext().WithPermissions(x => true);
        var handler = new SaveRequestHandler<TestRow>(context);
        using var connection = new MockDbConnection();
        var uow = new MockUnitOfWork(connection);
        connection.InterceptManipulateRow(args => 123L);
        var row = new TestRow() { Name = "Test" };
        var request = new SaveRequest<TestRow>()
        {
            Entity = row
        };
        handler.Create(uow, request);

        Assert.True(handler.IsCreate);
        Assert.False(handler.IsUpdate);
        Assert.NotNull(handler.Row);
        Assert.Null(handler.Old);

        Assert.Equal(context, handler.Context);
        Assert.Equal(context.Cache, handler.Cache);
        Assert.Equal(context.Localizer, handler.Localizer);
        Assert.Equal(context.Permissions, handler.Permissions);
        Assert.Equal(context.User, handler.User);
        Assert.Equal(connection, handler.Connection);
        Assert.Equal(uow, handler.UnitOfWork);
        Assert.Equal(request, handler.Request);
        Assert.NotNull(handler.Response);
        Assert.Equal(123L, handler.Response.EntityId);
        Assert.NotNull(handler.StateBag);

        ISaveRequestHandler ihandler = handler;
        Assert.True(ihandler.IsCreate);
        Assert.False(ihandler.IsUpdate);
        Assert.Equal(handler.Row, ihandler.Row);
        Assert.Null(ihandler.Old);

        Assert.Equal(handler.Context, ihandler.Context);
        Assert.Equal(handler.Connection, ihandler.Connection);
        Assert.Equal(handler.UnitOfWork, ihandler.UnitOfWork);
        Assert.Equal(handler.Request, ihandler.Request);
        Assert.Equal(handler.Response, ihandler.Response);
        Assert.Equal(handler.StateBag, ihandler.StateBag);
    }

    private class TestRow : IdNameRow<TestRow.RowFields>
    {
        public class RowFields : IdNameRowFields { }
    }
}
