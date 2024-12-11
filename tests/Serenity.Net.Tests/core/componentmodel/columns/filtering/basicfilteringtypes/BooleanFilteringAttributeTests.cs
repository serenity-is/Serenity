namespace Serenity.ComponentModel;

public class BooleanFilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Boolean()
    {
        var attribute = new BooleanFilteringAttribute();
        Assert.Equal("Boolean", attribute.FilteringType);
    }

    [Fact]
    public void FalseText_IsNull_ByDefault()
    {
        var attribute = new BooleanFilteringAttribute();
        Assert.Null(attribute.FalseText);
    }

    [Fact]
    public void FalseText_CanBeSet()
    {
        var attribute = new BooleanFilteringAttribute
        {
            FalseText = "False text"
        };
        Assert.Equal("False text", attribute.FalseText);
    }

    [Fact]
    public void FalseText_CanBeSet_ToNull()
    {
        var attribute = new BooleanFilteringAttribute
        {
            FalseText = null
        };
        Assert.Null(attribute.FalseText);
    }

    [Fact]
    public void TrueText_ShouldBeNull_ByDefault()
    {
        var attribute = new BooleanFilteringAttribute();
        Assert.Null(attribute.TrueText);
    }

    [Fact]
    public void TrueText_CanBeSet()
    {
        var attribute = new BooleanFilteringAttribute()
        {
            TrueText = "True Text"
        };
        Assert.Equal("True Text", attribute.TrueText);
    }

    [Fact]
    public void TrueText_CanBeSet_ToNull()
    {
        var attribute = new BooleanFilteringAttribute
        {
            TrueText = null
        };
        Assert.Null(attribute.TrueText);
    }

    [Fact]
    public void TrueText_ShouldNot_Affect_OtherProperties()
    {
        var attribute = new BooleanFilteringAttribute
        {
            TrueText = "True Text"
        };
        Assert.Equal("True Text", attribute.TrueText);
        Assert.Null(attribute.FalseText);
    }
}