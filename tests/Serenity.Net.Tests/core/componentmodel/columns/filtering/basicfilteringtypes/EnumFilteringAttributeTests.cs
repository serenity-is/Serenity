namespace Serenity.ComponentModel;

public class EnumFilteringAttributeTests()
{
    [Fact]
    public void FilteringType_ShouldBe_Enum()
    {
        var attribute = new EnumFilteringAttribute();
        Assert.Equal("Enum", attribute.FilteringType);
    }

    [Fact]
    public void EnumKey_IsNull_ByDefault()
    {
        var attribute = new EnumFilteringAttribute();
        Assert.Null(attribute.EnumKey);
    }

    [Fact]
    public void EnumKey_CanBeSet()
    {
        var attribute = new EnumFilteringAttribute
        {
            EnumKey = "EnumKey"
        };
        Assert.Equal("EnumKey", attribute.EnumKey);
    }

    [Fact]
    public void EnumKey_CanBeSet_ToNull()
    {
        var attribute = new EnumFilteringAttribute()
        {
            EnumKey = null,
        };
        Assert.Null(attribute.EnumKey);
    }
}