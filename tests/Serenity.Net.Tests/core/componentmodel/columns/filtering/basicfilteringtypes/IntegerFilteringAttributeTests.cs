namespace Serenity.ComponentModel;

public class IntegerFilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Integer()
    {
        var attribute = new IntegerFilteringAttribute();
        Assert.Equal("Integer",attribute.FilteringType);
    }
}
