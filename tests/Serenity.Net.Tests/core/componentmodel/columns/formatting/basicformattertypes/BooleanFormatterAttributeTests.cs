namespace Serenity.ComponentModel;

public class BooleanFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Boolean()
    {
        var attribute = new BooleanFormatterAttribute();
        Assert.Equal("Boolean", attribute.FormatterType);
    }

    [Fact]
    public void FalseText_CanBeSet()
    {
        var attribute = new BooleanFormatterAttribute
        {
            FalseText = "falseText"
        };
        Assert.Equal("falseText", attribute.FalseText);
    }

    [Fact]
    public void FalseText_CanBeSet_ToNull()
    {
        var attribute = new BooleanFormatterAttribute()
        {
            FalseText = null
        };
        Assert.Null(attribute.FalseText);
    }

    [Fact]
    public void FalseText_IsNull_ByDefault()
    {
        var attribute = new BooleanFormatterAttribute();
        Assert.Null(attribute.FalseText);
    }

    [Fact]
    public void TrueText_CanBeSet()
    {
        var attribute = new BooleanFormatterAttribute
        {
            TrueText = "trueText"
        };
        Assert.Equal("trueText", attribute.TrueText);
    }

    [Fact]
    public void TrueText_CanBeSet_ToNull()
    {
        var attribute = new BooleanFormatterAttribute()
        {
            TrueText = null
        };
        Assert.Null(attribute.TrueText);
    }

    [Fact]
    public void TrueText_IsNull_ByDefault()
    {
        var attribute = new BooleanFormatterAttribute();
        Assert.Null(attribute.TrueText);
    }
}