namespace Serenity.ComponentModel;

public class SummaryTypeAttributeTests()
{
    [Fact]
    public void Value_CanBePassed_ViaConstructor()
    {
        var attribute = new SummaryTypeAttribute(SummaryType.Max);
        Assert.Equal(SummaryType.Max, attribute.Value);
    }
}