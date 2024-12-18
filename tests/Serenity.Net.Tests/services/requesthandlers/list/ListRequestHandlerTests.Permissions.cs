namespace Serenity.Services;

public partial class ListRequestHandlerTests
{
    [Fact]
    public void Throws_If_User_Doesnt_Have_Permissions()
    {
        var context = new NullRequestContext().WithPermissions(x => x is not ReadPermission and not ServiceLookupPermission);
        var handler = new ListRequestHandler<TestRow>(context);
        Assert.Throws<ValidationError>(() => handler.List(new MockDbConnection(), new()));
    }

    [Fact]
    public void Passes_If_User_Has_ReadPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ReadPermission);
        var handler = new ListRequestHandler<TestRow>(context);
        handler.List(new MockDbConnection().InterceptExecuteReader(args => new MockDbDataReader()), new());
    }

    [Fact]
    public void Passes_If_User_Has_ServiceLookupPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ServiceLookupPermission);
        var handler = new ListRequestHandler<TestRow>(context);
        handler.List(new MockDbConnection().InterceptExecuteReader(args => new MockDbDataReader()), new());
    }

    [Fact]
    public void Should_Select_All_Fields_If_User_Has_Permissions()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull()).Columns.Select(x => x.ColumnName).ToList();
                Assert.Equal(TestRow.Fields.Count, columns.Count);
                Assert.Equal(columns, TestRow.Fields.Select(x => x.Name).ToList());
                return new MockDbDataReader();
            });

        new ListRequestHandler<TestRow>(new NullRequestContext()
            .WithPermissions(x => x is ReadPermission or ExtraSpecialFieldPermission))
            .List(connection, new());
        Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(1, connection.AllCallCount);
    }

    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var shouldSelect = TestRow.Fields
                    .Where(x => x.Name != TestRow.Fields.ExtraSpecialField.Name)
                    .Select(x => x.Name)
                    .ToList();
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull()).Columns.Select(x => x.ColumnName).ToList();
                Assert.Equal(shouldSelect.Count, columns.Count);
                Assert.Equal(columns, shouldSelect);
                return new MockDbDataReader();
            });


        new ListRequestHandler<TestRow>(new NullRequestContext()
            .WithPermissions(x => x is ReadPermission)).List(connection, new());
        Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(1, connection.AllCallCount);
    }

    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field_Even_If_Field_Is_In_IncludeColumns()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var shouldSelect = TestRow.Fields
                    .Where(x => x.Name != TestRow.Fields.ExtraSpecialField.Name)
                    .Select(x => x.Name)
                    .ToList();
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull()).Columns.Select(x => x.ColumnName).ToList();
                Assert.Equal(shouldSelect.Count, columns.Count);
                Assert.Equal(columns, shouldSelect);
                return new MockDbDataReader();
            });

        new ListRequestHandler<TestRow>(new NullRequestContext().WithPermissions(x => x is ReadPermission))
            .List(connection, new()
            {
                IncludeColumns = [TestRow.Fields.ExtraSpecialField.Name]
            });
        Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(1, connection.AllCallCount);
    }

    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field_Even_If_Field_Is_In_IncludeColumns_On_Fields_With_ReadPermission()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var shouldSelect = TestRow.Fields
                    .Where(x => x.IsLookup)
                    .Select(x => x.Name)
                    .ToList();
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull()).Columns.Select(x => x.ColumnName).ToList();
                Assert.Equal(shouldSelect.Count, columns.Count);
                Assert.Equal(columns, shouldSelect);
                return new MockDbDataReader();
            });

        new ListRequestHandler<TestRow>(new NullRequestContext()
                .WithPermissions(x => x is ServiceLookupPermission))
            .List(connection, new()
            {
                IncludeColumns = [TestRow.Fields.NormalField.Name]
            });
    }

    [Fact]
    public void Should_Throw_When_Request_Doesnt_Have_Any_Permissions()
    {
        var context = new NullRequestContext();
        var handler = new ListRequestHandler<TestRow>(context);

        Assert.Throws<ValidationError>(() => handler.List(new NullDbConnection(), new()));
    }
}