namespace Serenity.ComponentModel;

public class WidthAttributeTests()
{
    [Fact]
    public void Width_CanBePassed_ViaConstructor()
    {
        var attribute = new WidthAttribute(73);
        Assert.Equal(73, attribute.Value);
    }

    [Fact]
    public void MinAndMaxProperties_CanBeSet()
    {
        var attribute = new WidthAttribute(0)
        {
            Min = 10,
            Max = 100
        };

        Assert.Equal(10, attribute.Min);
        Assert.Equal(100, attribute.Max);
    }
}