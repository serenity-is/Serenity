namespace Serenity.Data;

public class RowValidationContextTests
{
    [Fact]
    public void Constructor_SetsConnection_AndLocalizer()
    {
        using var connection = new MockDbConnection();
        var row = new IdNameRow { ID = 1, Name = "Test" };

        var context = new RowValidationContext(connection, row, NullTextLocalizer.Instance);

        Assert.Same(connection, context.Connection);
        Assert.Same(NullTextLocalizer.Instance, context.Localizer);
        // Row is stored in a private field, verified indirectly via GetFieldValue tests
    }

    [Fact]
    public void GetFieldValue_ByPropertyName_ReturnsFieldValue()
    {
        using var connection = new MockDbConnection();
        var row = new IdNameRow { ID = 1, Name = "Test" };
        var context = new RowValidationContext(connection, row, NullTextLocalizer.Instance);

        Assert.Equal("Test", context.GetFieldValue("Name"));
    }

    [Fact]
    public void GetFieldValue_ByFieldName_UsesFieldLookupFallback()
    {
        using var connection = new MockDbConnection();
        // ComplexRow.Overriden property is backed by a field named "ManualName"
        var row = new ComplexRow { Overriden = "Value" };
        var context = new RowValidationContext(connection, row, NullTextLocalizer.Instance);

        Assert.Equal("Value", context.GetFieldValue("Overriden"));
        Assert.Equal("Value", context.GetFieldValue("ManualName"));
    }

    [Fact]
    public void GetFieldValue_UnknownName_ReturnsNull()
    {
        using var connection = new MockDbConnection();
        var row = new IdNameRow { Name = "Test" };
        var context = new RowValidationContext(connection, row, NullTextLocalizer.Instance);

        Assert.Null(context.GetFieldValue("DoesNotExist"));
    }

    [Fact]
    public void Value_GetSet_Works()
    {
        using var connection = new MockDbConnection();
        var row = new IdNameRow();
        var context = new RowValidationContext(connection, row, NullTextLocalizer.Instance);

        Assert.Null(context.Value);

        context.Value = "Test";
        Assert.Equal("Test", context.Value);

        context.Value = null;
        Assert.Null(context.Value);
    }
}
