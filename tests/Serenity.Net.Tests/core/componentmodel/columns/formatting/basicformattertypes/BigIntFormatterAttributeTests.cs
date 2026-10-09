namespace Serenity.ComponentModel;

public class BigIntFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_BigInt()
    {
        var attribute = new BigIntFormatterAttribute();
        Assert.Equal("BigInt", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new BigIntFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new BigIntFormatterAttribute()
        {
            DisplayFormat = "n0"
        };
        Assert.Equal("n0", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new BigIntFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }
}
