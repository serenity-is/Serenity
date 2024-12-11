namespace Serenity.ComponentModel;

public class NestedLocalTextsAttributeTests()
{
    [Fact]
    public void Constructor_IsAssignable_FromAttribute()
    {
        var attribute = new NestedLocalTextsAttribute();

        Assert.IsAssignableFrom<Attribute>(attribute);
    }

    [Fact]
    public void LanguageId_IsNull_ByDefault()
    {
        var attribute = new NestedLocalTextsAttribute();
        Assert.Null(attribute.LanguageID);
    }

    [Fact]
    public void LanguageId_CanBeSet()
    {
        var attribute = new NestedLocalTextsAttribute()
        {
            LanguageID = "LanguageId"
        };
        Assert.Equal("LanguageId", attribute.LanguageID);
    }

    [Fact]
    public void LanguageId_CanBeSet_ToNull()
    {
        var attribute = new NestedLocalTextsAttribute()
        {
            LanguageID = null
        };
        Assert.Null(attribute.LanguageID);
    }

    [Fact]
    public void Prefix_IsNull_ByDefault()
    {
        var attribute = new NestedLocalTextsAttribute();
        Assert.Null(attribute.Prefix);
    }

    [Fact]
    public void Prefix_CanBeSet()
    {
        var attribute = new NestedLocalTextsAttribute()
        {
            Prefix = "Prefix"
        };
        Assert.Equal("Prefix", attribute.Prefix);
    }

    [Fact]
    public void Prefix_CanBeSet_ToNull()
    {
        var attribute = new NestedLocalTextsAttribute()
        {
            Prefix = null
        };
        Assert.Null(attribute.Prefix);
    }
}