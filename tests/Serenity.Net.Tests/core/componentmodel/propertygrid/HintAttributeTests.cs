namespace Serenity.ComponentModel;

public class HintAttributeTests
{
    [Fact]
    public void Hint_CanBePassed_AsString()
    {
        var attribute = new HintAttribute("hint");
        Assert.Equal("hint", attribute.Hint);
    }

    [Fact]
    public void Hint_CanBePassed_AsNull()
    {
        var attribute = new HintAttribute(null);
        Assert.Null(attribute.Hint);
    }
}
