namespace Serenity.ComponentModel;

public class GroupOrderAttributeTests
{
    [Fact]
    public void GroupOrder_CanBePassed_AsInt()
    {
        var attribute = new GroupOrderAttribute(2);
        Assert.Equal(2, attribute.GroupOrder);
    }
}

