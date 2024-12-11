namespace Serenity.ComponentModel;

public class NestedPermissionKeysAttributeTests()
{
    [Fact]
    public void Constructor_IsAssignableFrom_Attribute()
    {
        var attribute = new NestedPermissionKeysAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }

    [Fact]
    public void LanguageID_IsNull_ByDefault()
    {
        var attribute = new NestedPermissionKeysAttribute();
        Assert.Null(attribute.LanguageID);
    }

    [Fact]
    public void LanguageID_CanBeSet()
    {
        var attribute = new NestedPermissionKeysAttribute()
        {
            LanguageID = "LanguageID"
        };
        Assert.Equal("LanguageID", attribute.LanguageID);
    }

    [Fact]
    public void LanguageID_CanBeSet_ToNull()
    {
        var attribute = new NestedPermissionKeysAttribute()
        {
            LanguageID = null
        };
        Assert.Null(attribute.LanguageID);
    }
}