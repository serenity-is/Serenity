namespace Serenity.ComponentModel;

public class QuickFilterAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new QuickFilterAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void Seperator_IsTrue_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Seperator_CanBeSet_ToFalse()
    {
        var attribute = new QuickFilterAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void CssClass_IsNull_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.Null(attribute.CssClass);
    }

    [Fact]
    public void CssClass_CanBeSet()
    {
        var attribute = new QuickFilterAttribute()
        {
            CssClass = "test-class"
        };
        Assert.Equal("test-class", attribute.CssClass);
    }

    [Fact]
    public void CssClass_CanBe_SetTo_NullValue()
    {
        var attribute = new QuickFilterAttribute()
        {
            CssClass = null
        };
        Assert.Null(attribute.CssClass);
    }

    [Fact]
    public void Separator_IsFalse_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.False(attribute.Separator);
    }

    [Fact]
    public void Separator_CanBeSet_ToTrue()
    {
        var attribute = new QuickFilterAttribute { Separator = true };
        Assert.True(attribute.Separator);
    }
}
