namespace Serenity.Data;

public class FileReadAccessAttributeTests
{
    [Fact]
    public void AllowBypass_DefaultsToTrue()
    {
        var attr = new FileReadAccessAttribute();
        Assert.True(attr.AllowBypass);
    }

    [Fact]
    public void PermissionOnly_DefaultsToFalse()
    {
        var attr = new FileReadAccessAttribute();
        Assert.False(attr.PermissionOnly);
    }

    [Fact]
    public void Permission_GetSet_Works()
    {
        var attr = new FileReadAccessAttribute { Permission = "*" };
        Assert.Equal("*", attr.Permission);
    }

    [Fact]
    public void AllowBypass_GetSet_Works()
    {
        var attr = new FileReadAccessAttribute { AllowBypass = false };
        Assert.False(attr.AllowBypass);
    }

    [Fact]
    public void PermissionOnly_GetSet_Works()
    {
        var attr = new FileReadAccessAttribute { PermissionOnly = true };
        Assert.True(attr.PermissionOnly);
    }
}

public class FileReadPermissionAttributeTests
{
    [Fact]
    public void Ctor_SetsPermission()
    {
        var attr = new FileReadPermissionAttribute("MyPermission");
        Assert.Equal("MyPermission", attr.Permission);
    }

    [Fact]
    public void Ctor_WithNullPermission_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FileReadPermissionAttribute(null));
    }
}