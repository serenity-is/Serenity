namespace Serenity.Navigation;

public class NavigationItemTests
{
    [Fact]
    public void Title_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            Title = "sometext"
        };
        Assert.Equal("sometext", attribute.Title);
    }

    [Fact]
    public void Title_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            Title = null
        };
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void FullPath_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.FullPath);
    }

    [Fact]
    public void FullPath_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            FullPath = "sometext"
        };
        Assert.Equal("sometext", attribute.FullPath);
    }

    [Fact]
    public void FullPath_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            FullPath = null
        };
        Assert.Null(attribute.FullPath);
    }

    [Fact]
    public void IconClass_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            IconClass = "sometext"
        };
        Assert.Equal("sometext", attribute.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            IconClass = null
        };
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void ItemClass_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void ItemClass_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            ItemClass = "sometext"
        };
        Assert.Equal("sometext", attribute.ItemClass);
    }

    [Fact]
    public void ItemClass_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            ItemClass = null
        };
        Assert.Null(attribute.ItemClass);
    }

    [Fact]
    public void Url_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void Url_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            Url = "sometext"
        };
        Assert.Equal("sometext", attribute.Url);
    }

    [Fact]
    public void Url_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            Url = null
        };
        Assert.Null(attribute.Url);
    }

    [Fact]
    public void Target_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.Title);
    }

    [Fact]
    public void Target_CanBeSet()
    {
        var attribute = new NavigationItem()
        {
            Target = "sometext"
        };
        Assert.Equal("sometext", attribute.Target);
    }

    [Fact]
    public void Target_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            Target = null
        };
        Assert.Null(attribute.Target);
    }

    [Fact]
    public void Parent_CanBeSet_ToNull()
    {
        var attribute = new NavigationItem()
        {
            Parent = null
        };
        Assert.Null(attribute.Target);
    }

    [Fact]
    public void Parent_IsNull_ByDefault()
    {
        var attribute = new NavigationItem();
        Assert.Null(attribute.Parent);
    }

    [Fact]
    public void Children_CanAddItems()
    {
        var testObject = new NavigationItem();
        var item = new NavigationItem();
        testObject.Children.Add(item);
        Assert.Contains(item, testObject.Children);
    }

    [Fact]
    public void Children_IsInitialized_AsEmptyList()
    {
        var testObject = new NavigationItem();
        Assert.NotNull(testObject.Children);
        Assert.Empty(testObject.Children);
    }

    [Fact]
    public void IsSection_CanBeSet_ToTrue()
    {
        var attribute = new NavigationItem()
        {
            IsSection = true
        };
        Assert.True(attribute.IsSection);
    }

    [Fact]
    public void IsSection_CanBeSet_ToFalse()
    {
        var attribute = new NavigationItem()
        {
            IsSection = false
        };
        Assert.False(attribute.IsSection);
    }
}