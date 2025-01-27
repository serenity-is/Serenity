namespace Serenity.Services;

public partial class SaveRequestHandler_Permission_Tests
{
    const string insertPermission = "Test:Insert";
    const string updatePermission = "Test:Update";

    [Fact]
    public void Create_Throws_If_User_DoesntHave_TheInsertPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is not insertPermission);
        var handler = new SaveRequestHandler<TestRow>(context);
        Assert.Throws<ValidationError>(() => handler.Create(new MockUnitOfWork(), new()
        {
            Entity = new() { Name = "Test" }
        }));
    }

    [Fact]
    public void Create_Allows_If_User_Has_TheInsertPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is insertPermission);
        var handler = new SaveRequestHandler<TestRow>(context);
        using var connection = new MockDbConnection();
        connection.InterceptManipulateRow(args =>
        {
            var insertRow = Assert.IsType<TestRow>(args.Row);
            Assert.False(args.Id.HasValue);
            Assert.True(args.GetNewId);
            Assert.True(insertRow.IsAssigned(TestRow.Fields.Name));
            Assert.Equal("Test", insertRow.Name);
            Assert.False(insertRow.IsAssigned(TestRow.Fields.Id));
            return 123;
        });
        var row = new TestRow() { Name = "Test" };
        handler.Create(new MockUnitOfWork(connection), new()
        {
            Entity = row
        });
        Assert.False(row.IsAssigned(TestRow.Fields.Id));

        Assert.Equal(1, connection.AllCallCount);
        Assert.Single(connection.ManipulateRowCalls);
    }

    [InsertPermission(insertPermission)]
    [UpdatePermission(updatePermission)]
    private class TestRow : IdNameRow<TestRow.RowFields>
    {
        public class RowFields : IdNameRowFields {}
    }
}
