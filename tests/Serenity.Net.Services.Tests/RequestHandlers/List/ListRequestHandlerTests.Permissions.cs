namespace Serenity.Tests.Services;

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
        handler.List(new MockDbConnection(), new());
    }

    [Fact]
    public void Passes_If_User_Has_ServiceLookupPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ServiceLookupPermission);
        var handler = new ListRequestHandler<TestRow>(context);
        handler.List(new MockDbConnection(), new());
    }

    [Fact]
    public void Should_Select_All_Fields_If_User_Has_Permissions()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ReadPermission or ExtraSpecialFieldPermission);
        var handler = new ListRequestHandler<TestRow>(context);

        handler.List(NewMockDbConnectionTestSelectedFields(TestRow.Fields.Select(x => x.Name)), new());
    }

    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ReadPermission);
        var handler = new ListRequestHandler<TestRow>(context);

        handler.List(NewMockDbConnectionTestSelectedFields(
            TestRow.Fields.Select(x => x.Name).Where(x => x != TestRow.Fields.ExtraSpecialField.Name)
        ), new());
    }

    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field_Even_If_Field_Is_In_IncludeColumns()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ReadPermission);
        var handler = new ListRequestHandler<TestRow>(context);

        handler.List(NewMockDbConnectionTestSelectedFields(
            TestRow.Fields.Select(x => x.Name).Where(x => x != TestRow.Fields.ExtraSpecialField.Name)
        ), new()
        {
            IncludeColumns = new() {TestRow.Fields.ExtraSpecialField.Name}
        });
    }
    
    [Fact]
    public void Should_Not_Select_A_Field_If_User_Doesnt_Have_Permission_For_That_Field_Even_If_Field_Is_In_IncludeColumns_On_Fields_With_ReadPermission()
    {
        var context = new NullRequestContext().WithPermissions(x => x is ServiceLookupPermission);
        var handler = new ListRequestHandler<TestRow>(context);

        handler.List(NewMockDbConnectionTestSelectedFields(
            TestRow.Fields.Where(x => x.IsLookup).Select(x => x.Name)
        ), new()
        {
            IncludeColumns = new() {TestRow.Fields.NormalField.Name}
        });
    }

    [Fact]
    public void Should_Throw_When_Request_Doesnt_Have_Any_Permissions()
    {
        var context = new NullRequestContext();
        var handler = new ListRequestHandler<TestRow>(context);
        
        Assert.Throws<ValidationError>(() => handler.List(new NullDbConnection(), new()));
    }

    private static IDbConnection NewMockDbConnectionTestSelectedFields(IEnumerable<string> containsFields)
    {
        var db = new MockDbConnection();
        db.OnExecuteReader(command =>
        {
            var enumerable = containsFields as string[] ?? containsFields.ToArray();

            foreach (var field in TestRow.Fields)
            {
                if (enumerable.Contains(field.Name))
                    Assert.Contains("T0." + field.Name, command.CommandText);
                else
                    Assert.DoesNotContain("T0." + field.Name, command.CommandText);
            }

            return new MockDbDataReader();
        });

        return db;
    }
}