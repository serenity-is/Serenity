namespace Serenity.Navigation;

public class NavigationMenuAttributeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithAllParameters()
    {
        int order = 1;
        string title = "MenuTitle";
        string icon = "icon-class";
        var attribute = new NavigationMenuAttribute(order, title, icon);
        Assert.Equal(order, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleAndIcon()
    {
        string title = "MenuTitle";
        string icon = "icon-class";
        var attribute = new NavigationMenuAttribute(title, icon);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleOnly()
    {
        string title = "MenuTitle";
        var attribute = new NavigationMenuAttribute(title);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Null(attribute.IconClass);
    }
}

