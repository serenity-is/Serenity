namespace Serenity.Services;

public class RepositoryTests
{
    private class TestBaseRepository(IRequestContext context) : BaseRepository(context)
    {
        public IRequestContext ContextPublic => Context;
        public ITwoLevelCache CachePublic => Cache;
        public ITextLocalizer LocalizerPublic => Localizer;
        public IPermissionService PermissionsPublic => Permissions;
        public ClaimsPrincipal? UserPublic => User;
    }

    public class BaseRepositoryTests
    {
        [Fact]
        public void Constructor_Throws_For_Null_Context()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBaseRepository(null));
        }

        [Fact]
        public void Exposes_Context_Services()
        {
            var context = new NullRequestContext();
            var repository = new TestBaseRepository(context);

            Assert.Same(context, repository.ContextPublic);
            Assert.Same(context.Cache, repository.CachePublic);
            Assert.Same(context.Localizer, repository.LocalizerPublic);
            Assert.Same(context.Permissions, repository.PermissionsPublic);
            Assert.Null(repository.UserPublic);
        }
    }

    public class DefaultRequestContextTests
    {
        private static DefaultRequestContext CreateContext(
            IBehaviorProvider? behaviors = null,
            ITwoLevelCache? cache = null,
            ITextLocalizer? localizer = null,
            IPermissionService? permissions = null,
            IUserAccessor? userAccessor = null)
        {
            return new DefaultRequestContext(
                behaviors ?? new NullBehaviorProvider(),
                cache ?? new NullTwoLevelCache(),
                localizer ?? NullTextLocalizer.Instance,
                permissions ?? new NullPermissions(),
                userAccessor ?? new NullUserAccessor());
        }

        [Fact]
        public void Constructor_Throws_For_Nulls()
        {
            var behaviors = new NullBehaviorProvider();
            var cache = new NullTwoLevelCache();
            var localizer = NullTextLocalizer.Instance;
            var permissions = new NullPermissions();
            var userAccessor = new NullUserAccessor();

            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRequestContext(null, cache, localizer, permissions, userAccessor));
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRequestContext(behaviors, null, localizer, permissions, userAccessor));
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRequestContext(behaviors, cache, null, permissions, userAccessor));
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRequestContext(behaviors, cache, localizer, null, userAccessor));
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRequestContext(behaviors, cache, localizer, permissions, null));
        }

        [Fact]
        public void Exposes_Provided_Services()
        {
            var behaviors = new NullBehaviorProvider();
            var cache = new NullTwoLevelCache();
            var localizer = NullTextLocalizer.Instance;
            var permissions = new NullPermissions();
            var context = CreateContext(behaviors, cache, localizer, permissions);

            Assert.Same(behaviors, context.Behaviors);
            Assert.Same(cache, context.Cache);
            Assert.Same(localizer, context.Localizer);
            Assert.Same(permissions, context.Permissions);
            Assert.Null(context.User);
        }

        [Fact]
        public void User_Returns_UserAccessor_User()
        {
            var context = CreateContext(userAccessor: new MockUserAccessor(() => TestUser.SysAdmin));

            Assert.NotNull(context.User);
        }
    }
}
