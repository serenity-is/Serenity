namespace Serenity.Navigation;

public class NavigationSectionAttributeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithAllParameters()
    {
        int order = 1;
        string title = "SectionTitle";
        string icon = "icon-class";
        var attribute = new NavigationSectionAttribute(order, title, icon);
        Assert.Equal(order, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleAndIcon()
    {
        string title = "SectionTitle";
        string icon = "icon-class";
        var attribute = new NavigationSectionAttribute(title, icon);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleOnly()
    {
        string title = "SectionTitle";
        var attribute = new NavigationSectionAttribute(title);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Null(attribute.IconClass);
    }
}

