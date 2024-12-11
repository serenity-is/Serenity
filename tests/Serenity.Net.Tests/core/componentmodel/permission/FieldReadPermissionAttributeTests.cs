namespace Serenity.Data;

public class FieldReadPermissionAttributeTests
{
    [Fact]
    public void Constructor_With_String()
    {
        var attribute = new FieldReadPermissionAttribute("a");
        Assert.Equal("a", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_String_String()
    {
        var attribute = new FieldReadPermissionAttribute("a", "a");
        Assert.Equal("a:a", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_String_String_String()
    {
        var attribute = new FieldReadPermissionAttribute("a", "b", "c");
        Assert.Equal("a:b:c", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum()
    {
        var attribute = new FieldReadPermissionAttribute(Serenity.IO.DeleteType.Delete);
        Assert.Equal("Delete", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum_Enum()
    {
        var attribute = new FieldReadPermissionAttribute(Serenity.IO.DeleteType.Delete, Serenity.IO.DeleteType.TryDelete);
        Assert.Equal("Delete:TryDelete", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum_Enum_Enum()
    {
        var attribute = new FieldReadPermissionAttribute(Serenity.IO.DeleteType.Delete, Serenity.IO.DeleteType.TryDelete, Serenity.IO.DeleteType.TryDeleteOrMark);
        Assert.Equal("Delete:TryDelete:TryDeleteOrMark", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int()
    {
        var attribute = new FieldReadPermissionAttribute(1);
        Assert.Equal("1", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int_Int()
    {
        var attribute = new FieldReadPermissionAttribute(1, 2);
        Assert.Equal("1:2", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int_Int_Int()
    {
        var attribute = new FieldReadPermissionAttribute(1, 2, 3);
        Assert.Equal("1:2:3", attribute.Permission);
    }

    [Fact]
    public void Constructor_WithNull()
    {
        var attribute = new FieldReadPermissionAttribute(null);
        Assert.Null(attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Null_Null()
    {
        var attribute = new FieldReadPermissionAttribute(null, null);
        Assert.Equal(":", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Null_Null_Null()
    {
        var attribute = new FieldReadPermissionAttribute(null, null, null);
        Assert.Equal("::", attribute.Permission);
    }
}
