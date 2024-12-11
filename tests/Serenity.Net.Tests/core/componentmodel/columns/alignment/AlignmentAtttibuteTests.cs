namespace Serenity.ComponentModel;

public class AlignmentAttributeTests
{
    [Fact]
    public void AlignCenterAttribute_ShouldHaveCenterValue()
    {
        var attr = new AlignCenterAttribute();
        Assert.Equal("center", attr.Value);
    }

    [Fact]
    public void AlignRightAttribute_ShouldHaveRightValue()
    {
        var attr = new AlignRightAttribute();
        Assert.Equal("right", attr.Value);
    }
}