namespace Serenity.Navigation;

public class NavigationItemAttributeTests
{
    public class MyNavigationItemAttribute(int order, string path, string url, object permission, string icon)
        : NavigationItemAttribute(order, path, url, permission, icon)
    {
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly_WithPathContainingSlash()
    {

        int order = 1;
        string path = "category/item";
        string url = "http://example.com";
        object permission = "read";
        string icon = "icon-class";
        var attribute = new MyNavigationItemAttribute(order, path, url, permission, icon);
        Assert.Equal("category", attribute.Category);
        Assert.Equal("item", attribute.Title);
        Assert.Equal(order, attribute.Order);
        Assert.Equal("read", attribute.Permission);
        Assert.Equal("icon-class", attribute.IconClass);
        Assert.Equal(url, attribute.Url);
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly_WithPathWithoutSlash()
    {

        int order = 2;
        string path = "item";
        string url = "http://example.com";
        object permission = "write";
        string icon = "icon-class";
        var attribute = new MyNavigationItemAttribute(order, path, url, permission, icon);
        Assert.Null(attribute.Category);
        Assert.Equal("item", attribute.Title);
        Assert.Equal(order, attribute.Order);
        Assert.Equal("write", attribute.Permission);
        Assert.Equal("icon-class", attribute.IconClass);
        Assert.Equal(url, attribute.Url);
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly_WithNullPath()
    {
        int order = 3;
        string path = null;
        string url = "http://example.com";
        object permission = null;
        string icon = "icon-class";
        var attribute = new MyNavigationItemAttribute(order, path, url, permission, icon);
        Assert.Null(attribute.Category);
        Assert.Equal("", attribute.Title);
        Assert.Equal(order, attribute.Order);
        Assert.Null(attribute.Permission);
        Assert.Equal("icon-class", attribute.IconClass);
        Assert.Equal(url, attribute.Url);
    }

    [Fact]
    public void Constructor_InitializesPropertiesCorrectly_WithEmptyPath()
    {
        int order = 4;
        string path = "";
        string url = "http://example.com";
        object permission = null;
        string icon = "icon-class";
        var attribute = new MyNavigationItemAttribute(order, path, url, permission, icon);
        Assert.Null(attribute.Category);
        Assert.Equal("", attribute.Title);
        Assert.Equal(order, attribute.Order);
        Assert.Null(attribute.Permission);
        Assert.Equal("icon-class", attribute.IconClass);
        Assert.Equal(url, attribute.Url);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Title = "sometext"
        };
        Assert.Equal("sometext", attribute.Title);
    }

    [Fact]
    public void Title_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Title = null
        };
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void FullPath_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            FullPath = "sometext"
        };
        Assert.Equal("sometext", attribute.FullPath);
    }

    [Fact]
    public void FullPath_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            FullPath = null
        };
        Assert.Null(attribute.FullPath);
    }

    [Fact]
    public void Category_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Category = "sometext"
        };
        Assert.Equal("sometext", attribute.Category);
    }

    [Fact]
    public void Category_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Category = null
        };
        Assert.Null(attribute.Category);
    }

    [Fact]
    public void IconClass_IsNull_ByDefault()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, null);
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            IconClass = "sometext"
        };
        Assert.Equal("sometext", attribute.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            IconClass = null
        };
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void ItemClass_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            ItemClass = "sometext"
        };
        Assert.Equal("sometext", attribute.ItemClass);
    }

    [Fact]
    public void ItemClass_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            ItemClass = null
        };
        Assert.Null(attribute.ItemClass);
    }


    [Fact]
    public void Url_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Url = "sometext"
        };
        Assert.Equal("sometext", attribute.Url);
    }

    [Fact]
    public void Url_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Url = null
        };
        Assert.Null(attribute.Url);
    }

    [Fact]
    public void Target_CanBeSet()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Target = "sometext"
        };
        Assert.Equal("sometext", attribute.Target);
    }

    [Fact]
    public void Target_CanBeSet_ToNull()
    {
        var attribute = new MyNavigationItemAttribute(1, "path", null, null, "icon")
        {
            Target = null
        };
        Assert.Null(attribute.Target);
    }
}

