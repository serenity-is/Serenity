namespace Serenity.ComponentModel;

public class CollapsibleAttributeTests
{
    [Fact]
    public void Value_ShouldBeTrue_ByDefault()
    {
        var attribute = new CollapsibleAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Collapsed_ShouldBeFalse_ByDefault()
    {
        var attribute = new CollapsibleAttribute();
        Assert.False(attribute.Collapsed);
    }

    [Fact]
    public void Collapsed_CanBeSet_ToTrue()
    {
        var attribute = new CollapsibleAttribute()
        {
            Collapsed = true
        };
        Assert.True(attribute.Collapsed);
    }
}
