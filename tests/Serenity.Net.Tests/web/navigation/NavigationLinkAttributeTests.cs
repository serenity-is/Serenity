namespace Serenity.Navigation;

public class NavigationLinkAttributeTests
{
    [Route("Northwind/[controller]")]
    private class CustomerController
    {
        [Route("Index")]
        public void Index()
        {
        }

        [Route("~/absolute")]
        public void Absolute()
        {
        }

        [NonAction]
        public void Hidden()
        {
        }
    }

    private class NoRouteController
    {
        public void Index()
        {
        }
    }

    private class ActionRouteOnlyController
    {
        [Route("Products/{id}")]
        public void Index()
        {
        }
    }

    [Route("Api")]
    private class AuthorizedController
    {
        [PageAuthorize("Action.Permission")]
        [FeatureBarrier("FeatureA", "FeatureB")]
        public void Index()
        {
        }

        public void NoAttributes()
        {
        }
    }

    [PageAuthorize("Controller.Permission")]
    private class ControllerAuthorized
    {
        public void Index()
        {
        }
    }

    [Fact]
    public void GetUrlFromController_Throws_For_Null_Or_Empty()
    {
        Assert.Throws<ArgumentNullException>(() => NavigationLinkAttribute.GetUrlFromController(null!, "Index"));
        Assert.Throws<ArgumentNullException>(() => NavigationLinkAttribute.GetUrlFromController(typeof(CustomerController), ""));
    }

    [Fact]
    public void GetUrlFromController_Throws_For_Unknown_Action()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NavigationLinkAttribute.GetUrlFromController(typeof(CustomerController), "Unknown"));
    }

    [Fact]
    public void GetUrlFromController_Throws_For_Non_Action()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NavigationLinkAttribute.GetUrlFromController(typeof(CustomerController), "Hidden"));
    }

    [Fact]
    public void GetUrlFromController_Throws_When_No_Route_Attribute()
    {
        Assert.Throws<InvalidOperationException>(() =>
            NavigationLinkAttribute.GetUrlFromController(typeof(NoRouteController), "Index"));
    }

    [Fact]
    public void GetUrlFromController_Combines_Controller_And_Action_Routes()
    {
        var url = NavigationLinkAttribute.GetUrlFromController(typeof(CustomerController), "Index");

        Assert.Equal("~/Northwind/Customer/Index", url);
    }

    [Fact]
    public void GetUrlFromController_Returns_Rooted_Action_Route()
    {
        var url = NavigationLinkAttribute.GetUrlFromController(typeof(CustomerController), "Absolute");

        Assert.Equal("~/absolute", url);
    }

    [Fact]
    public void GetUrlFromController_Uses_Action_Route_And_Removes_Parameters()
    {
        var url = NavigationLinkAttribute.GetUrlFromController(typeof(ActionRouteOnlyController), "Index");

        Assert.Equal("~/Products/", url);
    }

    [Fact]
    public void GetPermissionFromController_Throws_For_Null_Or_Empty()
    {
        Assert.Throws<ArgumentNullException>(() => NavigationLinkAttribute.GetPermissionFromController(null!, "Index"));
        Assert.Throws<ArgumentNullException>(() => NavigationLinkAttribute.GetPermissionFromController(typeof(CustomerController), ""));
    }

    [Fact]
    public void GetPermissionFromController_Throws_For_Unknown_Action()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NavigationLinkAttribute.GetPermissionFromController(typeof(CustomerController), "Unknown"));
    }

    [Fact]
    public void GetPermissionFromController_Reads_Action_Then_Controller_Permission()
    {
        Assert.Equal("Action.Permission",
            NavigationLinkAttribute.GetPermissionFromController(typeof(AuthorizedController), "Index"));
        Assert.Equal("Controller.Permission",
            NavigationLinkAttribute.GetPermissionFromController(typeof(ControllerAuthorized), "Index"));
        Assert.Null(NavigationLinkAttribute.GetPermissionFromController(typeof(CustomerController), "Index"));
    }

    [Fact]
    public void GetFeaturesFromController_Throws_For_Null_Or_Empty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NavigationLinkAttribute.GetFeaturesFromController(null!, "Index", out _));
        Assert.Throws<ArgumentNullException>(() =>
            NavigationLinkAttribute.GetFeaturesFromController(typeof(CustomerController), "", out _));
    }

    [Fact]
    public void GetFeaturesFromController_Throws_For_Unknown_Action()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NavigationLinkAttribute.GetFeaturesFromController(typeof(CustomerController), "Unknown", out _));
    }

    [Fact]
    public void GetFeaturesFromController_Returns_Features_And_RequireAny()
    {
        var features = NavigationLinkAttribute.GetFeaturesFromController(typeof(AuthorizedController), "Index", out var requireAny);

        Assert.Equal(["FeatureA", "FeatureB"], features);
        Assert.False(requireAny);

        Assert.Null(NavigationLinkAttribute.GetFeaturesFromController(typeof(CustomerController), "Index", out _));
    }

    [Fact]
    public void Constructor_With_Controller_Reads_Url_Permission_And_Features()
    {
        var attribute = new NavigationLinkAttribute(1, "Test", typeof(AuthorizedController));

        Assert.Equal("~/Api", attribute.Url);
        Assert.Equal("Action.Permission", attribute.Permission);
        Assert.Equal(["FeatureA", "FeatureB"], attribute.RequireFeatures);
    }

    [Fact]
    public void Constructor_With_Path_And_Controller_Reads_Url_Permission_And_Features()
    {
        var attribute = new NavigationLinkAttribute("Test", typeof(AuthorizedController));

        Assert.Equal("~/Api", attribute.Url);
        Assert.Equal("Action.Permission", attribute.Permission);
        Assert.Equal(["FeatureA", "FeatureB"], attribute.RequireFeatures);
    }

    [Fact]
    public void Constructor_With_Explicit_Values_Sets_Properties()
    {
        var attribute = new NavigationLinkAttribute(1, "A/B", "~/url", "P", "icon");

        Assert.Equal("A/B", attribute.FullPath);
        Assert.Equal("A", attribute.Category);
        Assert.Equal("B", attribute.Title);
        Assert.Equal("~/url", attribute.Url);
        Assert.Equal("P", attribute.Permission);
        Assert.Equal("icon", attribute.IconClass);
    }

    [Fact]
    public void Constructor_Without_Order_Uses_MaxValue()
    {
        var attribute = new NavigationLinkAttribute("A", "~/url", "P");

        Assert.Equal(int.MaxValue, attribute.Order);
    }
}
