namespace Serenity.ComponentModel;

public class MaxLengthAttributeTests
{
    [Fact]
    public void MaxLength_CanBePassed_AsInt()
    {
        var attribute = new MaxLengthAttribute(2);
        Assert.Equal(2, attribute.MaxLength);
    }
}

