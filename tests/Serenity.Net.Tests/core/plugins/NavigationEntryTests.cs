namespace Serenity.Plugins;

public class NavigationEntryTests
{
    [Fact]
    public void Order_CanBeSet_ToInt()
    {
        var entry = new NavigationEntry()
        {
            Order = 2
        };
        Assert.Equal(2, entry.Order);
    }

    [Fact]
    public void Action_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Action = "text"
        };
        Assert.Equal("text", entry.Action);
    }

    [Fact]
    public void Action_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Action = null
        };
        Assert.Null(entry.Action);
    }

    [Fact]
    public void Controller_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Controller = null
        };
        Assert.Null(entry.Controller);
    }

    [Fact]
    public void Controller_CanBeSet_ToTypeOfString()
    {
        var entry = new NavigationEntry()
        {

            Controller = typeof(string)
        };
        Assert.Equal(typeof(string), entry.Controller);
    }

    [Fact]
    public void Controller_CanBeSet_ToTypeOfInt()
    {
        var entry = new NavigationEntry()
        {
            Controller = typeof(int)
        };
        Assert.Equal(typeof(int), entry.Controller);
    }

    [Fact]
    public void Controller_CanBeSet_ToTypeOfObject()
    {
        var entry = new NavigationEntry()
        {
            Controller = typeof(object)
        };
        Assert.Equal(typeof(object), entry.Controller);
    }

    [Fact]
    public void Controller_CanBeSet_ToTypeOfDateTime()
    {
        var entry = new NavigationEntry()
        {
            Controller = typeof(DateTime)
        };
        Assert.Equal(typeof(DateTime), entry.Controller);
    }

    [Fact]
    public void Url_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Url = "text"
        };
        Assert.Equal("text", entry.Url);
    }

    [Fact]
    public void Url_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Url = null
        };
        Assert.Null(entry.Url);
    }

    [Fact]
    public void FullPath_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            FullPath = "text"
        };
        Assert.Equal("text", entry.FullPath);
    }

    [Fact]
    public void FullPath_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            FullPath = null
        };
        Assert.Null(entry.FullPath);
    }

    [Fact]
    public void Category_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Category = "text"
        };
        Assert.Equal("text", entry.Category);
    }

    [Fact]
    public void Category_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Category = null
        };
        Assert.Null(entry.Category);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Title = "text"
        };
        Assert.Equal("text", entry.Title);
    }

    [Fact]
    public void Title_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Title = null
        };
        Assert.Null(entry.Title);
    }

    [Fact]
    public void IconClass_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            IconClass = "text"
        };
        Assert.Equal("text", entry.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            IconClass = null
        };
        Assert.Null(entry.IconClass);
    }

    [Fact]
    public void ItemClass_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            ItemClass = "text"
        };
        Assert.Equal("text", entry.ItemClass);
    }

    [Fact]
    public void ItemClass_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            ItemClass = null
        };
        Assert.Null(entry.ItemClass);
    }

    [Fact]
    public void Permission_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Permission = "text"
        };
        Assert.Equal("text", entry.Permission);
    }

    [Fact]
    public void Permission_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Permission = null
        };
        Assert.Null(entry.Permission);
    }

    [Fact]
    public void Target_CanBeSet()
    {
        var entry = new NavigationEntry()
        {
            Target = "text"
        };
        Assert.Equal("text", entry.Target);
    }

    [Fact]
    public void Target_CanBeSet_ToNull()
    {
        var entry = new NavigationEntry()
        {
            Target = null
        };
        Assert.Null(entry.Target);
    }
}
