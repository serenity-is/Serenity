namespace Serenity.ComponentModel;

public class ImplicitPermissionAttributeTests { 

    [Fact]
    public void Value_CanBePassed_ToString()
    {
        var attribute = new ImplicitPermissionAttribute("sometext");
        Assert.Equal("sometext", attribute.Value);
    }
}
