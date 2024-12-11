namespace Serenity.Navigation;

public class NavigationGroupAttributeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithAllParameters()
    {
        int order = 1;
        string title = "GroupTitle";
        string icon = "icon-class";
        var attribute = new NavigationGroupAttribute(order, title, icon);
        Assert.Equal(order, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleAndIcon()
    {
        string title = "GroupTitle";
        string icon = "icon-class";
        var attribute = new NavigationGroupAttribute(title, icon);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Equal(icon, attribute.IconClass);
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithTitleOnly()
    {
        string title = "GroupTitle";
        var attribute = new NavigationGroupAttribute(title);
        Assert.Equal(int.MaxValue, attribute.Order);
        Assert.Equal(title, attribute.Title);
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void IncludeProperty_CanBeSetAndRetrieved()
    {
        var attribute = new NavigationGroupAttribute("Title");
        string[] includePaths = ["B/", "C/"];
        attribute.Include = includePaths;
        Assert.Equal(includePaths, attribute.Include);
    }

    [Fact]
    public void DefaultProperty_CanBeSetAndRetrieved()
    {
        var attribute = new NavigationGroupAttribute("Title");
        attribute.Default = true;
        Assert.True(attribute.Default);
    }
}
