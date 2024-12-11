namespace Serenity.Data;

public class ReadPermissionAttributeTests
{
    [Fact]
    public void Constructor_With_String()
    {
        var attribute = new ReadPermissionAttribute("a");
        Assert.Equal("a", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_String_String()
    {
        var attribute = new ReadPermissionAttribute("a", "a");
        Assert.Equal("a:a", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_String_String_String()
    {
        var attribute = new ReadPermissionAttribute("a", "b", "c");
        Assert.Equal("a:b:c", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum()
    {
        var attribute = new ReadPermissionAttribute(Serenity.IO.DeleteType.Delete);
        Assert.Equal("Delete", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum_Enum()
    {
        var attribute = new ReadPermissionAttribute(Serenity.IO.DeleteType.Delete, Serenity.IO.DeleteType.TryDelete);
        Assert.Equal("Delete:TryDelete", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Enum_Enum_Enum()
    {
        var attribute = new ReadPermissionAttribute(Serenity.IO.DeleteType.Delete, Serenity.IO.DeleteType.TryDelete, Serenity.IO.DeleteType.TryDeleteOrMark);
        Assert.Equal("Delete:TryDelete:TryDeleteOrMark", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int()
    {
        var attribute = new ReadPermissionAttribute(1);
        Assert.Equal("1", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int_Int()
    {
        var attribute = new ReadPermissionAttribute(1, 2);
        Assert.Equal("1:2", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Int_Int_Int()
    {
        var attribute = new ReadPermissionAttribute(1, 2, 3);
        Assert.Equal("1:2:3", attribute.Permission);
    }

    [Fact]
    public void Constructor_WithNull()
    {
        var attribute = new ReadPermissionAttribute(null);
        Assert.Null(attribute.Permission);

    }

    [Fact]
    public void Constructor_With_Null_Null()
    {
        var attribute = new ReadPermissionAttribute(null, null);
        Assert.Equal(":", attribute.Permission);
    }

    [Fact]
    public void Constructor_With_Null_Null_Null()
    {
        var attribute = new ReadPermissionAttribute(null, null, null);
        Assert.Equal("::", attribute.Permission);
    }
}
