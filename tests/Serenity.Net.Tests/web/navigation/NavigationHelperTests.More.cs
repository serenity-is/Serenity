namespace Serenity.Navigation;

public class NavigationHelperMoreTests
{
    public class TestNavigationSource : INavigationItemSource
    {
        public List<NavigationItemAttribute> GetItems() =>
            [new NavigationLinkAttribute(2, "FromSource", "~/s", null)];
    }

    [Fact]
    public void ByCategory_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => NavigationHelper.ByCategory(null!));
    }

    [Fact]
    public void ByCategory_Creates_Missing_Parent_Menus()
    {
        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[]
        {
            new NavigationLinkAttribute(1, "A/B", "~/b", null)
        });

        var parent = Assert.Single(lookup[""]);
        Assert.Equal("A", parent.Title);
        Assert.IsType<NavigationMenuAttribute>(parent);
    }

    [Fact]
    public void ByCategory_Does_Not_Create_Parent_When_Present()
    {
        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[]
        {
            new NavigationMenuAttribute(1, "A"),
            new NavigationLinkAttribute(2, "A/B", "~/b", null)
        });

        Assert.Single(lookup[""]);
        Assert.Equal("A", lookup[""].Single().Title);
    }

    [Fact]
    public void ByCategory_Moves_Included_Children_Into_Group()
    {
        var group = new NavigationGroupAttribute(1, "Group")
        {
            Include = ["A/"]
        };
        var link = new NavigationLinkAttribute(1, "A/B", "~/b", null);

        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[] { group, link });

        Assert.Contains(link, lookup["Group"]);
    }

    [Fact]
    public void ByCategory_Default_Group_Takes_Remaining_Siblings()
    {
        var group = new NavigationGroupAttribute(1, "Group") { Default = true };
        var link = new NavigationLinkAttribute(2, "B", "~/b", null);

        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[] { group, link });

        Assert.Contains(link, lookup["Group"]);
    }

    [Fact]
    public void ByCategory_Moves_Exact_Include_Path_Into_Group()
    {
        var group = new NavigationGroupAttribute(1, "Group") { Include = ["A/B"] };
        var link = new NavigationLinkAttribute(1, "A/B", "~/b", null);

        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[] { group, link });

        Assert.Contains(link, lookup["Group"]);
    }

    [Fact]
    public void ByCategory_Default_Group_Excludes_Included_Paths()
    {
        var group = new NavigationGroupAttribute(1, "Group") { Default = true, Include = ["B"] };
        var linkB = new NavigationLinkAttribute(2, "B", "~/b", null);
        var linkC = new NavigationLinkAttribute(3, "C", "~/c", null);

        var lookup = NavigationHelper.ByCategory(new NavigationItemAttribute[] { group, linkB, linkC });

        Assert.Contains(linkB, lookup[""]);
        Assert.Contains(linkC, lookup["Group"]);
    }

    [Fact]
    public void ByCategory_Creates_Chained_Missing_Parents()
    {
        var link = new NavigationLinkAttribute(1, "A/B/C", "~/c", null);

        var lookup = NavigationHelper.ByCategory([link]);

        Assert.Contains(lookup[""], x => x.Title == "A");
        Assert.Contains(lookup["A"], x => x.Title == "B");
    }

    [Fact]
    public void ByCategory_Sets_Group_Order_From_Children()
    {
        var group = new NavigationGroupAttribute(int.MaxValue, "Group") { Include = ["A/"] };
        var link = new NavigationLinkAttribute(5, "A/B", "~/b", null);

        NavigationHelper.ByCategory(new NavigationItemAttribute[] { group, link });

        Assert.Equal(5m, group.Order);
    }

    [Fact]
    public void ConvertToNavigationItems_Throws_For_Null_Permissions()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NavigationHelper.ConvertToNavigationItems(null!, NavigationHelper.ByCategory([]), null));
    }

    [Fact]
    public void ConvertToNavigationItems_Includes_Authorized_Sections()
    {
        var link = new NavigationLinkAttribute(1, "A", "~/a", "P");
        link.IconClass = " icon ";
        link.ItemClass = " item ";
        link.Target = " _blank ";

        var items = NavigationHelper.ConvertToNavigationItems(
            new MockPermissions(_ => true), NavigationHelper.ByCategory([link]), url => "/resolved" + url[1..]);

        var item = Assert.Single(items);
        Assert.Equal("A", item.Title);
        Assert.Equal("/resolved/a", item.Url);
        Assert.Equal("icon", item.IconClass);
        Assert.Equal("item", item.ItemClass);
        Assert.Equal("_blank", item.Target);
        Assert.False(item.IsSection);
    }

    [Fact]
    public void ConvertToNavigationItems_Excludes_Unauthorized_Leaf()
    {
        var link = new NavigationLinkAttribute(1, "A", "~/a", "P");

        var items = NavigationHelper.ConvertToNavigationItems(
            new MockPermissions(_ => false), NavigationHelper.ByCategory([link]), null);

        Assert.Empty(items);
    }

    [Fact]
    public void ConvertToNavigationItems_Keeps_Parent_With_Children()
    {
        var menu = new NavigationMenuAttribute(1, "A");
        var child = new NavigationLinkAttribute(1, "A/B", "~/b", null);

        var items = NavigationHelper.ConvertToNavigationItems(
            new MockPermissions(_ => false), NavigationHelper.ByCategory([menu, child]), null);

        var parent = Assert.Single(items);
        Assert.Equal("A", parent.Title);
        Assert.Single(parent.Children);
    }

    [Fact]
    public void ConvertToNavigationItems_Flags_Sections()
    {
        var section = new NavigationSectionAttribute(1, "Sec");
        var child = new NavigationLinkAttribute(1, "Sec/C", "~/c", null);

        var items = NavigationHelper.ConvertToNavigationItems(
            new MockPermissions(_ => true), NavigationHelper.ByCategory([section, child]), null);

        Assert.True(Assert.Single(items).IsSection);
    }

    [Fact]
    public void GetNavigationItems_Reads_Assembly_Attributes_And_Sources()
    {
        var attributes = new Attribute[] { new NavigationLinkAttribute(1, "FromAttribute", "~/a", null) };
        var typeSource = new MockTypeSource([typeof(TestNavigationSource)], attributes);
        var services = new ServiceCollection().BuildServiceProvider();

        var items = NavigationHelper.GetNavigationItems(
            new MockPermissions(_ => true), typeSource, services);

        Assert.Contains(items, x => x.Title == "FromAttribute");
        Assert.Contains(items, x => x.Title == "FromSource");
    }

    [Fact]
    public void GetNavigationItems_Filters_By_Feature_Toggles()
    {
        var link = new NavigationLinkAttribute(1, "Featured", "~/a", null)
        {
            RequireFeatures = ["Feature"]
        };
        var typeSource = new MockTypeSource(new Attribute[] { link });
        var services = new ServiceCollection()
            .AddSingleton<IFeatureToggles>(new MockFeatureToggles { IsEnabledCallback = _ => false })
            .BuildServiceProvider();

        var items = NavigationHelper.GetNavigationItems(
            new MockPermissions(_ => true), typeSource, services);

        Assert.Empty(items);
    }

    [Fact]
    public void GetNavigationItems_Applies_Filter()
    {
        var link = new NavigationLinkAttribute(1, "A", "~/a", null);
        var typeSource = new MockTypeSource(new Attribute[] { link });
        var services = new ServiceCollection().BuildServiceProvider();

        var items = NavigationHelper.GetNavigationItems(
            new MockPermissions(_ => true), typeSource, services, filter: _ => false);

        Assert.Empty(items);
    }
}
