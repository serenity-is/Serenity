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

    [Fact]
    public void Range_IsNotSet_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.False(attribute.Range);
        Assert.Null(attribute.GetRange());
    }

    [Fact]
    public void Range_CanBeSet_ToFalse_And_ToTrue()
    {
        var attribute = new QuickFilterAttribute { Range = false };
        Assert.False(attribute.Range);
        Assert.Equal(false, attribute.GetRange());

        attribute.Range = true;
        Assert.True(attribute.Range);
        Assert.Equal(true, attribute.GetRange());
    }
}
