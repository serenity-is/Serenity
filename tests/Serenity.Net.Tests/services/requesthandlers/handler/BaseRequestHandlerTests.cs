namespace Serenity.Services;

public class BaseRequestHandlerTests
{
    private class TestBaseRequestHandler(IRequestContext context) : BaseRequestHandler(context)
    {
        public IRequestContext ContextPublic => Context;
        public ITwoLevelCache CachePublic => Cache;
        public ITextLocalizer LocalizerPublic => Localizer;
        public IPermissionService PermissionsPublic => Permissions;
        public ClaimsPrincipal? UserPublic => User;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Context()
    {
        Assert.Throws<ArgumentNullException>(() => new TestBaseRequestHandler(null));
    }

    [Fact]
    public void Exposes_Context_Services()
    {
        var context = new NullRequestContext();
        var handler = new TestBaseRequestHandler(context);

        Assert.Same(context, handler.ContextPublic);
        Assert.Same(context.Cache, handler.CachePublic);
        Assert.Same(context.Localizer, handler.LocalizerPublic);
        Assert.Same(context.Permissions, handler.PermissionsPublic);
        Assert.Null(handler.UserPublic);
    }
}
