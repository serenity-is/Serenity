namespace Serenity.ComponentModel;

public class DecimalFilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Decimal()
    {
        var attribute = new DecimalFilteringAttribute();
        Assert.Equal("Decimal", attribute.FilteringType);
    }
}