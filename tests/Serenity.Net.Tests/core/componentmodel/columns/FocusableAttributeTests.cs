namespace Serenity.ComponentModel;

public class FocusableAttributeTests
{
    [Fact]
    public void Value_DefaultsToTrue()
    {
        var attr = new FocusableAttribute();
        Assert.True(attr.Value);
    }

    [Fact]
    public void Value_IsSet()
    {
        Assert.True(new FocusableAttribute(true).Value);
        Assert.False(new FocusableAttribute(false).Value);
    }
}

public class ShowSelectionAttributeTests
{
    [Fact]
    public void Value_DefaultsToTrue()
    {
        var attr = new ShowSelectionAttribute();
        Assert.True(attr.Value);
    }

    [Fact]
    public void Value_IsSet()
    {
        Assert.True(new ShowSelectionAttribute(true).Value);
        Assert.False(new ShowSelectionAttribute(false).Value);
    }
}

public class TabbableAttributeTests
{
    [Fact]
    public void Value_DefaultsToTrue()
    {
        var attr = new TabbableAttribute();
        Assert.True(attr.Value);
    }

    [Fact]
    public void Value_IsSet()
    {
        Assert.True(new TabbableAttribute(true).Value);
        Assert.False(new TabbableAttribute(false).Value);
    }
}