namespace Serenity.ComponentModel;

public class FilteringTypeAttributeTests
{
    [Fact]
    public void FilteringType_IsPassed_ViaConstructorArgument()
    {
        var attribute = new FilteringTypeAttribute("someType");
        Assert.Equal("someType", attribute.FilteringType);
    }

    [Fact]
    public void FilteringType_CanBePassed_AsNull()
    {
        var attribute = new FilteringTypeAttribute(null);
        Assert.Null(attribute.FilteringType);
    }

    [Fact]
    public void SetParams_Method_CanBeCalled()
    {
        var attribute = new FilteringTypeAttribute("expectedType");
        var formatterParams = new Dictionary<string, object>();
        attribute.SetParams(formatterParams);
    }
}


