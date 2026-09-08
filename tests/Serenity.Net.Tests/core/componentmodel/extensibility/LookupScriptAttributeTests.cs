namespace Serenity.ComponentModel;

public class LookupScriptAttributeTests()
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullType()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute((Type)null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullString()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute((string)null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForEmptyString()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute(""));
    }

    [Fact]
    public void Constructor_CanBeCall_WithoutKey()
    {
        var attribute = new LookupScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new LookupScriptAttribute("key");
        Assert.Equal("key", attribute.Key);
    }

    [Module("Northwind")]
    private class ModuleRow
    {
    }

    private class EntitiesRow
    {
    }

    private class MyLookup
    {
    }

    [LookupScript("MyLookup")]
    private class LookupType
    {
    }

    private class PlainType
    {
    }

    [Fact]
    public void AutoLookupKeyFor_UsesModuleAttribute()
    {
        Assert.Equal("Northwind.Module", LookupScriptAttribute.AutoLookupKeyFor(typeof(ModuleRow)));
    }

    [Fact]
    public void AutoLookupKeyFor_RemovesEntitiesSuffix()
    {
        Assert.Equal("ComponentModel.Entities", LookupScriptAttribute.AutoLookupKeyFor(typeof(EntitiesRow)));
    }

    [Fact]
    public void AutoLookupKeyFor_RemovesLookupSuffix()
    {
        Assert.Equal("ComponentModel.My", LookupScriptAttribute.AutoLookupKeyFor(typeof(MyLookup)));
    }

    [Fact]
    public void Constructor_WithLookupType_SetsKeyAndLookupType()
    {
        var attr = new LookupScriptAttribute(typeof(LookupType));
        Assert.Equal("MyLookup", attr.Key);
        Assert.Equal(typeof(LookupType), attr.LookupType);
    }

    [Fact]
    public void Constructor_WithPlainType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LookupScriptAttribute(typeof(PlainType)));
    }

    [Fact]
    public void Permission_GetSet_Works()
    {
        var attr = new LookupScriptAttribute { Permission = "*" };
        Assert.Equal("*", attr.Permission);
    }

    [Fact]
    public void Expiration_GetSet_Works()
    {
        var attr = new LookupScriptAttribute { Expiration = 60 };
        Assert.Equal(60, attr.Expiration);
    }

    [Fact]
    public void LookupType_GetSet_Works()
    {
        var attr = new LookupScriptAttribute { LookupType = typeof(string) };
        Assert.Equal(typeof(string), attr.LookupType);
    }
}