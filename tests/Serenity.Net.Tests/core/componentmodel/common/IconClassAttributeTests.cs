namespace Serenity.ComponentModel;

public class IconClassAttributeTests
{
    [Fact]
    public void Value_IsSet()
    {
        var attr = new IconClassAttribute("fa fa-user");
        Assert.Equal("fa fa-user", attr.Value);
    }
}

public class FixedWidthAttributeTests
{
    [Fact]
    public void Ctor_SetsValueMinAndMax()
    {
        var attr = new FixedWidthAttribute(100);
        Assert.Equal(100, attr.Value);
        Assert.Equal(100, attr.Min);
        Assert.Equal(100, attr.Max);
    }
}