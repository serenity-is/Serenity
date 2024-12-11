namespace Serenity.ComponentModel;

public class EditorOptionAttributeTests
{
    [Fact]
    public void Key_CanBePassed_AsString_And_Value_CanBePassed_AsInt()
    {
        var attribute = new EditorOptionAttribute("sometext", 2);
        Assert.Equal("sometext", attribute.Key);
        Assert.Equal(2, attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsEnum()
    {
        var attribute = new EditorOptionAttribute("sometext", Serenity.IO.DeleteType.Delete);
        Assert.Equal(Serenity.IO.DeleteType.Delete, attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new EditorOptionAttribute("sometext", "somevalue");
        Assert.Equal("somevalue", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_Null()
    {
        var attribute = new EditorOptionAttribute("somekey", null);
        Assert.Null(attribute.Value);
    }
}
