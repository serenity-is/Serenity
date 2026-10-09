namespace Serenity.ComponentModel;

public class Int64FilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Int64()
    {
        var attribute = new Int64FilteringAttribute();
        Assert.Equal("Int64", attribute.FilteringType);
    }
}
