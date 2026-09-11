namespace Serenity.Web;

public class PageAuthorizeAttributeTests
{
    [ReadPermission("Test:Read")]
    private class RowWithReadPermission
    {
    }

    [DisplayName("No permission")]
    private class RowWithoutPermission
    {
    }

    private class TestPageAuthorizeAttribute(Type sourceType, params Type[] attributeTypes)
        : PageAuthorizeAttribute(sourceType, attributeTypes)
    {
    }

    private static (IResourceFilter filter, DefaultHttpContext httpContext) CreateFilter(
        PageAuthorizeAttribute attr, bool isLoggedIn, Func<string, bool>? hasPermission = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPermissionService>(new MockPermissions(p => hasPermission?.Invoke(p) ?? false));
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            User = new ClaimsPrincipal(new ClaimsIdentity([], isLoggedIn ? "Authenticated" : ""))
        };
        var filter = (IResourceFilter)((IFilterFactory)attr).CreateInstance(httpContext.RequestServices);
        return (filter, httpContext);
    }

    private static ResourceExecutingContext CreateExecutingContext(DefaultHttpContext httpContext, IResourceFilter filter)
    {
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [filter], []);
    }

    [Fact]
    public void Default_Constructor_Has_No_Permission()
    {
        Assert.Null(new PageAuthorizeAttribute().Permission);
    }

    [Fact]
    public void Constructor_With_Permission_Sets_Permission()
    {
        Assert.Equal("Test:Permission", new PageAuthorizeAttribute("Test:Permission").Permission);
    }

    [Fact]
    public void Constructor_With_Null_Permission_Sets_Null()
    {
        Assert.Null(new PageAuthorizeAttribute((object?)null).Permission);
    }

    [Fact]
    public void Constructor_With_Module_And_Permission_Joins_With_Colon()
    {
        Assert.Equal("Test:Permission", new PageAuthorizeAttribute("Test", "Permission").Permission);
    }

    [Fact]
    public void Constructor_With_Module_Submodule_And_Permission_Joins_With_Colons()
    {
        Assert.Equal("Test:Sub:Permission",
            new PageAuthorizeAttribute("Test", "Sub", "Permission").Permission);
    }

    [Fact]
    public void Constructor_With_SourceType_Reads_ReadPermission()
    {
        Assert.Equal("Test:Read", new PageAuthorizeAttribute(typeof(RowWithReadPermission)).Permission);
    }

    [Fact]
    public void Constructor_With_SourceType_And_AttributeTypes_Reads_First_Match()
    {
        var attr = new TestPageAuthorizeAttribute(typeof(RowWithReadPermission),
            typeof(DisplayNameAttribute), typeof(ReadPermissionAttribute));

        Assert.Equal("Test:Read", attr.Permission);
    }

    [Fact]
    public void Constructor_With_SourceType_Throws_When_SourceType_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestPageAuthorizeAttribute(null!, typeof(ReadPermissionAttribute)));
    }

    [Fact]
    public void Constructor_With_SourceType_Throws_When_AttributeTypes_Are_Empty()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestPageAuthorizeAttribute(typeof(RowWithReadPermission)));
    }

    [Fact]
    public void Constructor_With_SourceType_Throws_When_Attribute_Is_Not_PermissionAttributeBase()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TestPageAuthorizeAttribute(typeof(RowWithReadPermission), typeof(DisplayNameAttribute)));
    }

    [Fact]
    public void Constructor_With_SourceType_Throws_When_No_Matching_Attribute()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TestPageAuthorizeAttribute(typeof(RowWithoutPermission), typeof(ReadPermissionAttribute)));
    }

    [Fact]
    public void Equals_Compares_Permission()
    {
        var first = new PageAuthorizeAttribute("A");
        var second = new PageAuthorizeAttribute("A");
        var third = new PageAuthorizeAttribute("B");

        Assert.True(first.Equals(second));
        Assert.False(first.Equals(third));
        Assert.False(first.Equals(null));
        Assert.False(first.Equals("A"));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void OnResourceExecuting_Challenges_Anonymous_User()
    {
        var attr = new PageAuthorizeAttribute();
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: false);
        var context = CreateExecutingContext(httpContext, filter);

        filter.OnResourceExecuting(context);

        Assert.IsType<ChallengeResult>(context.Result);
    }

    [Fact]
    public void OnResourceExecuting_Passes_LoggedIn_User_Without_Permission()
    {
        var attr = new PageAuthorizeAttribute();
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: true);
        var context = CreateExecutingContext(httpContext, filter);

        filter.OnResourceExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnResourceExecuting_Passes_When_User_Has_Permission()
    {
        var attr = new PageAuthorizeAttribute("Test:Permission");
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: true, p => p == "Test:Permission");
        var context = CreateExecutingContext(httpContext, filter);

        filter.OnResourceExecuting(context);

        Assert.Null(context.Result);
    }

    [Fact]
    public void OnResourceExecuting_Forbids_LoggedIn_User_Without_Permission()
    {
        var attr = new PageAuthorizeAttribute("Test:Permission");
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: true, _ => false);
        var context = CreateExecutingContext(httpContext, filter);

        filter.OnResourceExecuting(context);

        Assert.IsType<ForbidResult>(context.Result);
    }

    [Fact]
    public void OnResourceExecuting_Challenges_Anonymous_User_Without_Permission()
    {
        var attr = new PageAuthorizeAttribute("Test:Permission");
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: false, _ => false);
        var context = CreateExecutingContext(httpContext, filter);

        filter.OnResourceExecuting(context);

        Assert.IsType<ChallengeResult>(context.Result);
    }

    [Fact]
    public void OnResourceExecuted_Does_Nothing()
    {
        var attr = new PageAuthorizeAttribute();
        var (filter, httpContext) = CreateFilter(attr, isLoggedIn: true);
        var executedContext = new ResourceExecutedContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()), [filter]);

        filter.OnResourceExecuted(executedContext);

        Assert.Null(executedContext.Result);
    }
}
