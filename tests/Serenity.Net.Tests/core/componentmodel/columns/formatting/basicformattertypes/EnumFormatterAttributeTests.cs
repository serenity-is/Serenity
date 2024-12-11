namespace Serenity.ComponentModel;

public class EnumFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Enum()
    {
        var attribute = new EnumFormatterAttribute();
        Assert.Equal("Enum", attribute.FormatterType);
    }

    [Fact]
    public void EnumKey_CanBeSet()
    {
        var attribute = new EnumFormatterAttribute()
        {
            EnumKey = null
        };
        Assert.Null(attribute.EnumKey);
    }

    [Fact]
    public void EnumKey_CanBeSet_ToNull()
    {
        var attribute = new EnumFormatterAttribute()
        {
            EnumKey = null
        };
        Assert.Null(attribute.EnumKey);
    }

    [Fact]
    public void EnumKey_IsNull_ByDefault()
    {
        var attribute = new EnumFormatterAttribute();
        Assert.Null(attribute.EnumKey);
    }
}