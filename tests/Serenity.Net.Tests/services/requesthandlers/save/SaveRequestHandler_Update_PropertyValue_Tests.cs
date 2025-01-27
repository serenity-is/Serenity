namespace Serenity.Services;

public partial class SaveRequestHandler_Update_PropertyValue_Tests
{
    [Fact]
    public void Public_PropertyValues_AreAsExpected()
    {
        var context = new NullRequestContext().WithPermissions(x => true);
        var handler = new SaveRequestHandler<TestRow>(context);
        using var connection = new MockDbConnection();
        var uow = new MockUnitOfWork(connection);
        var existing = new TestRow() { Id = 123, Name = "OldTest" };
        connection.InterceptExecuteReader(args => new MockDbDataReader(new
        {
            Id = 123,
            Name = "OldTest"
        }));
        connection.InterceptManipulateRow(args => 1);
        var row = new TestRow() { Id = 123, Name = "NewTest" };
        var request = new SaveRequest<TestRow>()
        {
            Entity = row
        };
        handler.Update(uow, request);

        Assert.False(handler.IsCreate);
        Assert.True(handler.IsUpdate);
        
        Assert.NotNull(handler.Row);
        Assert.Equal(123, handler.Row.Id);
        Assert.Equal("NewTest", handler.Row.Name);
        
        Assert.NotNull(handler.Old);
        Assert.Equal(123, handler.Old.Id);
        Assert.Equal("OldTest", handler.Old.Name);

        Assert.Equal(context, handler.Context);
        Assert.Equal(context.Cache, handler.Cache);
        Assert.Equal(context.Localizer, handler.Localizer);
        Assert.Equal(context.Permissions, handler.Permissions);
        Assert.Equal(context.User, handler.User);
        Assert.Equal(connection, handler.Connection);
        Assert.Equal(uow, handler.UnitOfWork);
        Assert.Equal(request, handler.Request);
        Assert.NotNull(handler.Response);
        Assert.Equal(123, handler.Response.EntityId);
        Assert.NotNull(handler.StateBag);

        ISaveRequestHandler ihandler = handler;
        Assert.False(ihandler.IsCreate);
        Assert.True(ihandler.IsUpdate);
        Assert.Equal(handler.Row, ihandler.Row);
        Assert.Equal(handler.Old, ihandler.Old);

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
